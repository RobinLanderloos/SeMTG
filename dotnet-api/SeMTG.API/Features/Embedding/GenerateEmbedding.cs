using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Embedding;
using SeMTG.API.Qdrant;

namespace SeMTG.API.Features.Embedding;

public static class GenerateEmbedding
{
	public static void MapGenerateEmbedding(this IEndpointRouteBuilder builder)
	{
		builder.MapPost("/generate/{cardId}", GenerateEmbeddingAsync);
	}

	private static async Task<Results<Ok, NotFound, InternalServerError<string>>> GenerateEmbeddingAsync(Guid cardId,
		[FromServices] CardEmbedder embedder,
		[FromServices] ApplicationDbContext dbContext,
		[FromServices] QdrantService qdrantService)
	{
		try
		{
			var card = await dbContext.Cards.Include(card => card.Editions).FirstOrDefaultAsync(x => x.Id == cardId);

			if (card == null)
			{
				return TypedResults.NotFound();
			}

			var latestEdition = card.GetLatestEdition();

			if (latestEdition == null)
			{
				return TypedResults.NotFound();
			}

			var vector = await embedder.GenerateEmbedding(latestEdition);
			card.SetVector(vector);
			await dbContext.SaveChangesAsync();

			await qdrantService.UpsertCardAsync(latestEdition, vector);

			return TypedResults.Ok();
		}
		catch (Exception ex)
		{
			return TypedResults.InternalServerError(ex.Message);
		}
	}
}