using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Embedding;
using SeMTG.API.Models;
using SeMTG.API.Qdrant;

namespace SeMTG.API.Features.ScryfallImport;

public class ScryfallCardsImporter(IEmbeddingService embeddingService, QdrantService qdrantService, ILogger<ScryfallCardsImporter> logger, IServiceScopeFactory serviceScopeFactory)
{
	private const int BatchSize = 64;

	public async Task Import(string path)
	{
		try
		{
			logger.LogInformation("Starting import for path {Path}", path);

			var cards = await LoadAndFilterCardsAsync(path);

			var chunks = SplitIntoChunks(cards);

			await ProcessChunksAsync(chunks);

			logger.LogInformation("Import completed successfully.");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Import failed.");
		}
	}

	private async Task<List<ScryfallCardObject>> LoadAndFilterCardsAsync(string path)
	{
		logger.LogInformation("Loading cards from {Path}", path);

		var cards = await LoadCardsAsync(path);

		logger.LogDebug("Loaded {Count} cards", cards.Count);

		var filteredCards = cards
			.Where(card =>
				!string.IsNullOrEmpty(card.OracleText) &&
				!(card.TypeLine?.Contains("token", StringComparison.OrdinalIgnoreCase) ?? false))
			.ToList();

		logger.LogInformation("Filtered down to {Count} cards", filteredCards.Count);

		return filteredCards;
	}

	private List<ScryfallCardObject[]> SplitIntoChunks(List<ScryfallCardObject> cards)
	{
		var chunks = cards.Chunk(BatchSize).ToList();

		logger.LogInformation("Split into {Count} chunks (BatchSize={BatchSize})", chunks.Count, BatchSize);

		return chunks;
	}

	private async Task ProcessChunksAsync(List<ScryfallCardObject[]> chunks)
	{
		for (var i = 0; i < chunks.Count; i++)
		{
			var chunk = chunks[i];

			logger.LogInformation("Processing chunk {ChunkIndex}/{ChunkCount} with {CardCount} cards", i, chunks.Count, chunk.Length);

			await using var scope = serviceScopeFactory.CreateAsyncScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			await AddCardsToDatabaseAsync(dbContext, chunk);

			await UpsertVectorsAsync(dbContext, chunk);

			logger.LogInformation("Finished processing chunk {ChunkIndex}/{ChunkCount}", i, chunks.Count);
		}
	}

	private async Task AddCardsToDatabaseAsync(ApplicationDbContext dbContext,
		ScryfallCardObject[] cards)
	{
		foreach (var card in cards)
		{
			var existingCard = await dbContext.ScryfallCards.FirstOrDefaultAsync(x => x.Id == card.Id);
			if (existingCard != null)
			{
				logger.LogDebug("Card {CardName} already exists, skipping", card.Name);
				continue;
			}

			await dbContext.ScryfallCards.AddAsync(card);
		}

		logger.LogInformation("Saving new cards to database");

		await dbContext.SaveChangesAsync();
	}

	private async Task UpsertVectorsAsync(ApplicationDbContext dbContext, ScryfallCardObject[] cards)
	{
		var texts = cards
			.Select(card => $"{card.Name} {card.TypeLine} {card.OracleText}")
			.ToList();

		logger.LogInformation("Embedding cards");

		var vectors = await embeddingService.EmbedBatchAsync(texts);

		for (var i = 0; i < vectors.Count; i++)
		{
			logger.LogDebug("Upserting card {CardName} to Qdrant", cards[i].Name);
			var card = cards[i];

			var existingQdrantInfo = await dbContext.QdrantInfo.FirstOrDefaultAsync(x => x.ScryfallCardObjectId == card.Id);
			if (existingQdrantInfo != null)
			{
				logger.LogDebug("Card {CardName} already has Qdrant info, skipping", card.Name);
				continue;
			}

			var qdrantInfo = new QdrantInfo(card.Id, vectors[i]);
			await dbContext.QdrantInfo.AddAsync(qdrantInfo);
			await qdrantService.UpsertCardAsync(cards[i], vectors[i]);
			await dbContext.SaveChangesAsync();
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