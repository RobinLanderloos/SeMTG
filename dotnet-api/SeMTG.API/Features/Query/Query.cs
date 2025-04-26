using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SeMTG.API.Embedding;
using SeMTG.API.Qdrant;

namespace SeMTG.API.Features.Query;

public static class Query
{
	public static void MapQuery(this IEndpointRouteBuilder builder)
	{
		builder.MapGet("/query", async Task<Ok<List<CardPayload>>> (IEmbeddingService embeddingService, QdrantService qdrantService, [FromQuery] string query, [FromQuery] ulong limit = 10) =>
		{
			var queryVector = await embeddingService.EmbedAsync(query);

			var hits = await qdrantService.SearchAsync(queryVector, limit);

			return TypedResults.Ok(hits);
		});
	}
}