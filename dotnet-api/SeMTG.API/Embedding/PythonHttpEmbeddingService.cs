using System.Text.Json;

namespace SeMTG.API.Embedding;

public class PythonHttpEmbeddingService : IEmbeddingService
{
	private readonly ILogger<PythonHttpEmbeddingService> _logger;

	public PythonHttpEmbeddingService(ILogger<PythonHttpEmbeddingService> logger)
	{
		_logger = logger;
	}

	public async Task<List<float[]>> EmbedBatchAsync(List<string> texts)
	{
		_logger.LogInformation("Embedding {Count} texts", texts.Count);

		using var http = new HttpClient();
		var request = new { texts, batch_size = texts.Count };
		var response = await http.PostAsJsonAsync("http://localhost:5000/embed", request);
		response.EnsureSuccessStatusCode();
		var json = await response.Content.ReadFromJsonAsync<JsonElement>();
		var vectors = json.GetProperty("vectors").EnumerateArray()
			.Select(x => x.EnumerateArray().Select(y => y.GetSingle()).ToArray()).ToList();

		_logger.LogInformation("Embedding completed");
		return vectors;
	}

	public async Task<float[]> EmbedAsync(string text)
	{
		_logger.LogInformation("Embedding text {Text}", text);

		var vector = await EmbedBatchAsync([text]);

		_logger.LogInformation("Embedding completed");
		return vector[0];
	}
}