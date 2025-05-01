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

public static class Search
{
	public static void MapSearch(this IEndpointRouteBuilder builder)
	{
		builder.MapGet("", async Task<Ok<SearchResult>> (IEmbeddingService embeddingService,
			QdrantService qdrantService,
			ApplicationDbContext dbContext,
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


			var queryVector = await embeddingService.EmbedAsync(query);

			var scoredPoints = await qdrantService.SearchAsync(queryVector, (ulong)limit);

			var searchResultCards = new List<SearchResultCard>();
			foreach (var scoredPoint in scoredPoints)
			{
				var card = await scoredPoint.ToSearchResultCard(dbContext);

				if (card == null)
				{
					continue;
				}

				searchResultCards.Add(card);
			}

			return TypedResults.Ok(new SearchResult(searchResultCards));
		});
	}

	private static async Task<SearchResultCard?> ToSearchResultCard(this ScoredPoint scoredPoint,
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