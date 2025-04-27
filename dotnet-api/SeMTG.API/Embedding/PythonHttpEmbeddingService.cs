using System.Text.Json;
using SeMTG.API.Models;

namespace SeMTG.API.Embedding;

public class PythonHttpEmbeddingService : IEmbeddingService
{
	private readonly ILogger<PythonHttpEmbeddingService> _logger;

	public PythonHttpEmbeddingService(ILogger<PythonHttpEmbeddingService> logger)
	{
		_logger = logger;
	}

	public async Task<List<float[]>> EmbedBatchAsync(List<CardEdition> cardEditions)
	{
		_logger.LogInformation("Embedding {Count} card editions", cardEditions.Count);

		var textsToEmbed = cardEditions.Select(GetTextToEmbed).ToList();

		var vectors = await EmbedBatchAsync(textsToEmbed);

		_logger.LogInformation("Embedding card editions batch completed");

		return vectors;
	}

	public async Task<List<float[]>> EmbedBatchAsync(List<string> texts)
	{
		_logger.LogInformation("Embedding {Count} texts", texts.Count);

		using var http = new HttpClient();

		var request = new { texts = texts, batch_size = texts.Count };
		var response = await http.PostAsJsonAsync("http://localhost:5000/embed", request);
		response.EnsureSuccessStatusCode();

		var json = await response.Content.ReadFromJsonAsync<JsonElement>();
		var vectors = json.GetProperty("vectors").EnumerateArray()
			.Select(x => x.EnumerateArray().Select(y => y.GetSingle()).ToArray())
			.ToList();

		return vectors;
	}

	public async Task<float[]> EmbedAsync(CardEdition cardEdition)
	{
		_logger.LogInformation("Embedding single card edition: {CardEdition}", cardEdition);

		var vector = await EmbedBatchAsync([cardEdition]);
		return vector[0];
	}

	public async Task<float[]> EmbedAsync(string text)
	{
		var vector = await EmbedBatchAsync([text]);
		return vector[0];
	}


	private static string GetTextToEmbed(CardEdition cardEdition)
	{
		return $"{cardEdition.Name} {cardEdition.TypeLine} {cardEdition.OracleText}";
	}
}