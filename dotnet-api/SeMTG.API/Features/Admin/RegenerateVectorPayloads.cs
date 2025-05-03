using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Qdrant;

namespace SeMTG.API.Features.Admin;

public class RegenerateVectorPayloads
{
	public static void MapRegenerateVectorPayloads(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/regenerate-vector-payloads", RegenerateVectorPayloadsAsync);
	}

	private static async Task RegenerateVectorPayloadsAsync(ApplicationDbContext dbContext, QdrantService qdrantService, ILogger<RegenerateVectorPayloads> logger)
	{
		logger.LogInformation("Regenerating vector payloads");
		var cardEditionsCount = await dbContext.Cards.CountAsync();

		var chunkSize = 500;
		var currentChunk = 0;
		var totalChunks = (int)Math.Ceiling(cardEditionsCount / (double)chunkSize);
		logger.LogInformation("Splitting into {TotalChunks} chunks", totalChunks);

		for (var i = 0; i < totalChunks; i++)
		{
			logger.LogInformation("Processing chunk {ChunkIndex}/{TotalChunks}", i, totalChunks);
			var cards = await dbContext.Cards.OrderBy(x => x.Name).Skip(currentChunk * chunkSize).Take(chunkSize).ToListAsync();
			foreach (var cardEdition in cards.Select(card => card.GetLatestEdition()))
			{
				if (cardEdition == null)
				{
					continue;
				}

				await qdrantService.UpdatePayloadAsync(cardEdition);
			}
		}
	}
}