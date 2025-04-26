using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Embedding;
using SeMTG.API.Models;
using SeMTG.API.Qdrant;

namespace SeMTG.API.Features.ScryfallImport;

public class ScryfallCardsImporter(IEmbeddingService embeddingService, QdrantService qdrantService, ILogger<ScryfallCardsImporter> logger, IServiceScopeFactory serviceScopeFactory)
{
	private const int BatchSize = 1024;

	public async Task Import(string path)
	{
		try
		{
			logger.LogInformation("Starting import for path {Path}", path);
			var cards = await LoadCardsAsync(path);
			logger.LogDebug("Loaded {Count} cards", cards.Count);


			// Filter out tokens and empty cards
			var filteredCards = cards.Where(card =>
				!string.IsNullOrEmpty(card.OracleText) &&
				!(card.TypeLine?.Contains("token", StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

			logger.LogDebug("Filtered down to {Count} cards", filteredCards.Count);

			var chunks = filteredCards.Chunk(BatchSize).ToList();

			logger.LogDebug("Split into {Count} chunks", chunks.Count);

			// Process in batches
			for (var i = 0; i < chunks.Count; i++)
			{
				logger.LogDebug("Processing chunk {Index} with {Count} cards", i, chunks[i].Length);

				using var scope = serviceScopeFactory.CreateScope();
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

				foreach (var card in chunks[i])
				{
					var existingCard = await dbContext.ScryfallCards.FirstOrDefaultAsync(x => x.Id == card.Id);
					if (existingCard != null)
					{
						logger.LogDebug("Card {CardName} already exists", card.Name);
						continue;
					}

					await dbContext.ScryfallCards.AddAsync(card);
				}

				logger.LogDebug("Saving changes for chunk {Index}", i);
				await dbContext.SaveChangesAsync();

				// Use the oracle text as the vector
				var texts = chunks[i].Select(card => card.OracleText).ToList();
				var vectors = await embeddingService.EmbedBatchAsync(texts);

				for (var y = 0; y < vectors.Count; y++)
				{
					logger.LogDebug("Upserting card {CardName} in chunk {ChunkIndex}", chunks[i][y].Name, i);
					await qdrantService.UpsertCardAsync(chunks[i][y], vectors[y]);
				}

				logger.LogDebug("Chunk {Index} completed", i);
			}

			logger.LogInformation("Import completed");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Import failed");
		}
	}

	private async Task<List<ScryfallCardObject>> LoadCardsAsync(string path)
	{
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
		};

		await using var fs = File.OpenRead(path);
		var cards = await JsonSerializer.DeserializeAsync<List<ScryfallCardObject>>(fs, options);

		if (cards == null) throw new ArgumentException("Failed to deserialize cards");

		return cards;
	}
}