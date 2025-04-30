using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qdrant.Client.Grpc;
using SeMTG.API.Database;
using SeMTG.API.Embedding;
using SeMTG.API.Models;
using SeMTG.API.Qdrant;

namespace SeMTG.API.Features.Search;

public static class Search
{
	public static void MapSearch(this IEndpointRouteBuilder builder)
	{
		builder.MapGet("", async Task<Ok<List<Hit>>> (IEmbeddingService embeddingService,
			QdrantService qdrantService,
			ApplicationDbContext dbContext,
			[FromQuery] string query,
			[FromQuery] ulong limit = 10) =>
		{
			var queryVector = await embeddingService.EmbedAsync(query);

			var scoredPoints = await qdrantService.SearchAsync(queryVector, limit);

			var hitsWithCards = new List<Hit>();

			foreach (var scoredPoint in scoredPoints)
			{
				var hit = await scoredPoint.ToHit(dbContext);

				if (hit == null)
				{
					continue;
				}

				hitsWithCards.Add(hit);
			}

			return TypedResults.Ok(hitsWithCards);
		});
	}

	private static async Task<Hit?> ToHit(this ScoredPoint scoredPoint,
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

		var result = new CardResult(card, card.Vector);
		var hit = new Hit(result, scoredPoint.Score);
		return hit;
	}

	public record Hit(CardResult Card, float Score);

	public record CardResult(Guid Id, string Name, float[]? Vectors, IReadOnlyCollection<CardEditionResult> Editions)
	{
		public CardResult(Card card,
			float[]? vectors) : this(card.Id, card.Name, vectors, card.Editions.Select(edition => new CardEditionResult(edition)).ToList())
		{
		}
	}

	public record CardEditionResult(Guid Id, string Name, DateOnly ReleasedAt, string ScryfallUri, string ManaCost, string TypeLine, string OracleText, string ImageUri)
	{
		public CardEditionResult(CardEdition edition) : this(edition.Id, edition.Name, edition.ReleasedAt, edition.ScryfallUri, edition.ManaCost, edition.TypeLine, edition.OracleText, edition.ImageUris.Large)
		{
		}
	}
}