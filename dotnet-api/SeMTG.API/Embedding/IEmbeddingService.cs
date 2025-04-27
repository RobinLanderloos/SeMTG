using SeMTG.API.Models;

namespace SeMTG.API.Embedding;

public interface IEmbeddingService
{
	Task<List<float[]>> EmbedBatchAsync(List<CardEdition> cardEditions);
	Task<List<float[]>> EmbedBatchAsync(List<string> texts);
	Task<float[]> EmbedAsync(CardEdition cardEdition);
	Task<float[]> EmbedAsync(string text);
}