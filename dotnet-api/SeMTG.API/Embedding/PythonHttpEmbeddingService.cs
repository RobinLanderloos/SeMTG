using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using SeMTG.API.Models;

namespace SeMTG.API.Embedding;

public class PythonHttpEmbeddingService : IEmbeddingService
{
    private readonly ILogger<PythonHttpEmbeddingService> _logger;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;
    private readonly string _host;
    private readonly int _port;

    // Cache key prefix to avoid collisions with other cached items
    private const string CacheKeyPrefix = "Embedding_";

    public PythonHttpEmbeddingService(
        ILogger<PythonHttpEmbeddingService> logger,
        IMemoryCache memoryCache,
        IConfiguration configuration)
    {
        _logger = logger;
        _cache = memoryCache;
        _port = int.Parse(configuration["PythonEmbedder:Port"] ?? throw new InvalidOperationException("PythonEmbedder:Port is not set"));
        _host = configuration["PythonEmbedder:Host"] ?? throw new InvalidOperationException("PythonEmbedder:Host is not set");

        // Default cache options - items expire after 24 hours
        _cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(24))
            .SetSize(1); // Each entry counts as 1 unit for cache size limiting
    }

    public async Task<List<float[]>> EmbedBatchAsync(List<CardEdition> cardEditions)
    {
        _logger.LogInformation("Embedding {Count} card editions", cardEditions.Count);

        // Split into cached and uncached items
        var cachedResults = new List<(int Index, float[] Vector)>();
        var uncachedItems = new List<(int Index, string Text)>();

        for (int i = 0; i < cardEditions.Count; i++)
        {
            var text = GetTextToEmbed(cardEditions[i]);
            string cacheKey = $"{CacheKeyPrefix}{text.GetHashCode()}";

            if (_cache.TryGetValue(cacheKey, out float[] cachedVector))
            {
                _logger.LogDebug("Cache hit for card edition: {CardEdition}", cardEditions[i]);
                cachedResults.Add((i, cachedVector));
            }
            else
            {
                uncachedItems.Add((i, text));
            }
        }

        // If all items were cached, return immediately
        if (uncachedItems.Count == 0)
        {
            _logger.LogInformation("All card editions found in cache");
            var allCachedVectors = new List<float[]>(cardEditions.Count);
            for (int i = 0; i < cardEditions.Count; i++)
            {
                allCachedVectors.Add(null); // Placeholder
            }

            foreach (var (index, vector) in cachedResults)
            {
                allCachedVectors[index] = vector;
            }

            return allCachedVectors;
        }

        // Get embeddings for uncached items
        var textsToEmbed = uncachedItems.Select(item => item.Text).ToList();
        var newVectors = await EmbedBatchAsync(textsToEmbed);

        // Cache the newly computed embeddings
        for (int i = 0; i < uncachedItems.Count; i++)
        {
            var (originalIndex, text) = uncachedItems[i];
            var vector = newVectors[i];

            string cacheKey = $"{CacheKeyPrefix}{text.GetHashCode()}";
            _cache.Set(cacheKey, vector, _cacheOptions);
        }

        // Combine cached and new results in original order
        var result = new List<float[]>(cardEditions.Count);
        for (int i = 0; i < cardEditions.Count; i++)
        {
            result.Add(null); // Placeholder
        }

        foreach (var (index, vector) in cachedResults)
        {
            result[index] = vector;
        }

        for (int i = 0; i < uncachedItems.Count; i++)
        {
            var (originalIndex, _) = uncachedItems[i];
            result[originalIndex] = newVectors[i];
        }

        _logger.LogInformation("Embedding card editions batch completed (Cache hits: {CacheHits}, Misses: {CacheMisses})",
            cachedResults.Count, uncachedItems.Count);

        return result;
    }

    public async Task<List<float[]>> EmbedBatchAsync(List<string> texts)
    {
        _logger.LogInformation("Embedding {Count} texts", texts.Count);

        // Apply caching for text embeddings too
        var cachedResults = new List<(int Index, float[] Vector)>();
        var uncachedItems = new List<(int Index, string Text)>();

        for (int i = 0; i < texts.Count; i++)
        {
            string cacheKey = $"{CacheKeyPrefix}{texts[i].GetHashCode()}";

            if (_cache.TryGetValue(cacheKey, out float[] cachedVector))
            {
                _logger.LogDebug("Cache hit for text at index {Index}", i);
                cachedResults.Add((i, cachedVector));
            }
            else
            {
                uncachedItems.Add((i, texts[i]));
            }
        }

        // If all items were cached, return immediately
        if (uncachedItems.Count == 0)
        {
            _logger.LogInformation("All texts found in cache");
            var allCachedVectors = new List<float[]>(texts.Count);
            for (int i = 0; i < texts.Count; i++)
            {
                allCachedVectors.Add(null); // Placeholder
            }

            foreach (var (index, vector) in cachedResults)
            {
                allCachedVectors[index] = vector;
            }

            return allCachedVectors;
        }

        // Process uncached items
        using var http = new HttpClient();

        var textsToEmbed = uncachedItems.Select(item => item.Text).ToList();
        var request = new { texts = textsToEmbed, batch_size = textsToEmbed.Count };
        var response = await http.PostAsJsonAsync($"http://{_host}:{_port}/embed", request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var newVectors = json.GetProperty("vectors").EnumerateArray()
            .Select(x => x.EnumerateArray().Select(y => y.GetSingle()).ToArray())
            .ToList();

        // Cache the newly computed embeddings
        for (int i = 0; i < uncachedItems.Count; i++)
        {
            var (_, text) = uncachedItems[i];
            var vector = newVectors[i];

            string cacheKey = $"{CacheKeyPrefix}{text.GetHashCode()}";
            _cache.Set(cacheKey, vector, _cacheOptions);
        }

        // Combine cached and new results in original order
        var result = new List<float[]>(texts.Count);
        for (int i = 0; i < texts.Count; i++)
        {
            result.Add(null); // Placeholder
        }

        foreach (var (index, vector) in cachedResults)
        {
            result[index] = vector;
        }

        for (int i = 0; i < uncachedItems.Count; i++)
        {
            var (originalIndex, _) = uncachedItems[i];
            result[originalIndex] = newVectors[i];
        }

        _logger.LogInformation("Embedding texts completed (Cache hits: {CacheHits}, Misses: {CacheMisses})",
            cachedResults.Count, uncachedItems.Count);

        return result;
    }

    public async Task<float[]> EmbedAsync(CardEdition cardEdition)
    {
        string text = GetTextToEmbed(cardEdition);
        string cacheKey = $"{CacheKeyPrefix}{text.GetHashCode()}";

        if (_cache.TryGetValue(cacheKey, out float[] cachedVector))
        {
            _logger.LogDebug("Cache hit for card edition: {CardEdition}", cardEdition);
            return cachedVector;
        }

        _logger.LogInformation("Embedding single card edition: {CardEdition}", cardEdition);

        var vector = await EmbedBatchAsync([cardEdition]);
        return vector[0];
    }

    public async Task<float[]> EmbedAsync(string text)
    {
        string cacheKey = $"{CacheKeyPrefix}{text.GetHashCode()}";

        if (_cache.TryGetValue(cacheKey, out float[] cachedVector))
        {
            _logger.LogDebug("Cache hit for text: {Text}", text);
            return cachedVector;
        }

        var vector = await EmbedBatchAsync([text]);
        return vector[0];
    }

    private static string GetTextToEmbed(CardEdition cardEdition)
    {
        return $"{cardEdition.Name} {cardEdition.TypeLine} {cardEdition.OracleText}";
    }
}