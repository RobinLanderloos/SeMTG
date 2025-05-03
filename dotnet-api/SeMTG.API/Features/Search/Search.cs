using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qdrant.Client.Grpc;
using SeMTG.API.Database;
using SeMTG.API.Embedding;
using SeMTG.API.Features.Shared.Models;
using SeMTG.API.Models;
using SeMTG.API.Qdrant;

namespace SeMTG.API.Features.Search;

public class Search
{
	public static void MapSearch(IEndpointRouteBuilder builder)
	{
		builder.MapGet("", async Task<Ok<SearchResult>> (IEmbeddingService embeddingService,
			QdrantService qdrantService,
			ApplicationDbContext dbContext,
			ILogger<Search> logger,
			[FromQuery] string query,
			[FromQuery] int limit = 10) =>
		{
			if (string.IsNullOrEmpty(query))
			{
				var results = dbContext
					.Cards
					.Include(c => c.Editions)
					.ThenInclude(e => e.ImageUris)
					.Take(limit)
					.AsEnumerable()
					.Select(card => new SearchResultCard(card.Id, card.Name, 1, card.Editions.LastOrDefault() == null ? new ImageUrisDto("", "", "", "", "", "") : new ImageUrisDto(card.Editions.Last().ImageUris)))
					.ToList();

				return TypedResults.Ok(new SearchResult(results));
			}
			logger.LogInformation("Searching for {Query}", query);
			var queryVector = await embeddingService.EmbedAsync(query);

			logger.LogInformation("Searching vector DB");
			var scoredPoints = await qdrantService.SearchAsync(queryVector, (ulong)limit);
			logger.LogInformation("Found {Count} results", scoredPoints.Count);

			logger.LogInformation("Converting results to search result cards");
			var searchResultCards = new List<SearchResultCard>();
			foreach (var scoredPoint in scoredPoints)
			{
				var card = await ToSearchResultCard(scoredPoint, dbContext);

				if (card == null)
				{
					continue;
				}

				searchResultCards.Add(card);
			}

			logger.LogInformation("Returning search result");
			return TypedResults.Ok(new SearchResult(searchResultCards));
		});
	}

	private static async Task<SearchResultCard?> ToSearchResultCard(ScoredPoint scoredPoint,
		ApplicationDbContext dbContext)
	{
		var pointId = Guid.Parse(scoredPoint.Id.Uuid);
		var card = await dbContext.Cards
			.Include(c => c.Editions)
			.ThenInclude(ed => ed.ImageUris)
			.FirstOrDefaultAsync(card => card.Id == pointId);

		if (card == null)
		{
			return null;
		}

		return new SearchResultCard(card.Id, card.Name, scoredPoint.Score, new(card.Editions.Last().ImageUris));
	}
}

public record SearchResult(IReadOnlyCollection<SearchResultCard> Cards);

public record SearchResultCard(Guid Id, string Name, float score, ImageUrisDto ImageUris);