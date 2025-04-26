using SeMTG.API.Embedding;
using SeMTG.API.Qdrant;

namespace SeMTG.API.Features.Admin;

public static class RecreateCollection
{
	public static void MapRecreateCollection(this IEndpointRouteBuilder builder)
	{
		builder.MapPost("/admin/recreate-collection", RecreateCollectionAsync);
	}

	public static async Task RecreateCollectionAsync(IEmbeddingService embeddingService, QdrantService qdrantService)
	{
		await qdrantService.RecreateCollectionAsync();
	}
}