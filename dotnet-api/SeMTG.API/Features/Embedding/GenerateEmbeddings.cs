using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Embedding;
using SeMTG.API.Qdrant;

namespace SeMTG.API.Features.Embedding;

public class GenerateEmbeddings
{
	public static void MapGenerateEmbeddings(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/generate-embeddings", GenerateEmbeddingsAsync);
	}

	private static async Task GenerateEmbeddingsAsync([FromServices] IEmbeddingService embeddingService,
		[FromServices] ApplicationDbContext dbContext,
		[FromServices] QdrantService qdrantService,
		[FromServices] ILogger<GenerateEmbeddings> logger,
		CancellationToken cancellationToken,
		int batchSize = 64,
		bool onlyMissing = true)
	{
		var cardsQueryable = dbContext.Cards.Include(c => c.Editions)
			.Where(card => card.Editions.Any());

		if (onlyMissing)
		{
			cardsQueryable = cardsQueryable.Where(card => card.Vector == null);
		}
		var cards = await cardsQueryable.ToListAsync(cancellationToken: cancellationToken);
		logger.LogInformation("Found {Count} cards with missing vectors", cards.Count);

		var chunks = cards.Chunk(batchSize).ToList();
		logger.LogInformation("Split into {Count} chunks", chunks.Count);

		var i = 0;
		foreach (var chunk in chunks)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				logger.LogWarning("Embedding cancelled");
				return;
			}
			logger.LogInformation("Processing chunk {Index}/{ChunksLength}", i, chunks.Count);
			var latestEditions = chunk.Select(card => card.GetLatestEdition()!).ToList();
			var vectors = await embeddingService.EmbedBatchAsync(latestEditions);

			var zipped = latestEditions.Zip(vectors).ToList();
			foreach (var (edition, vector) in zipped)
			{
				edition.Card.SetVector(vector);
				dbContext.Cards.Update(edition.Card);
			}

			await qdrantService.UpsertCardsAsync(zipped);
			await dbContext.SaveChangesAsync(cancellationToken);
			i++;
		}
	}
}