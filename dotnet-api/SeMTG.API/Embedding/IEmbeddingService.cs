namespace SeMTG.API.Embedding;

public interface IEmbeddingService
{
	Task<List<float[]>> EmbedBatchAsync(List<string> texts);
	Task<float[]> EmbedAsync(string text);
}