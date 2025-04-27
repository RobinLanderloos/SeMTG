using SeMTG.API.Models;

namespace SeMTG.API.Embedding;

public class CardEmbedder(IEmbeddingService embeddingService, ILogger<CardEmbedder> logger)
{
	public async Task<float[]> GenerateEmbedding(CardEdition cardEdition)
	{
		var vector = await embeddingService.EmbedAsync(cardEdition);
		return vector;
	}
}