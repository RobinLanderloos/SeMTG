using Qdrant.Client;
using Qdrant.Client.Grpc;
using SeMTG.API.Models;

namespace SeMTG.API.Qdrant;

public class QdrantService
{
	private const string CollectionName = "mtg-cards";
	private const string Host = "localhost";
	private const int Port = 6334;
	private const ulong VectorSize = 768;
	private const Distance VectorDistance = Distance.Cosine;

	private readonly ILogger<QdrantService> _logger;
	private readonly QdrantClient _client;

	public QdrantService(ILogger<QdrantService> logger)
	{
		_logger = logger;
		_client = CreateClient();
	}

	public async Task<List<CardPayload>> SearchAsync(float[] vector,
		ulong limit,
		SearchQuality searchQuality = SearchQuality.Normal)
	{
		var result = await _client.SearchAsync(CollectionName, vector, limit: limit, searchParams: new SearchParams()
		{
			HnswEf = (ulong)searchQuality
		});

		return result.Select(hit => new CardPayload(hit)).ToList();
	}

	public async Task UpsertCardsAsync(List<(CardEdition Edition, float[] Vector)> cardEditionsAndVectors)
	{
		var points = cardEditionsAndVectors.Select(x => x.Edition.ToPointStruct(x.Vector)).ToList();

		await _client.UpsertAsync(CollectionName, points);
	}

	public async Task UpsertCardAsync(CardEdition cardEdition,
		float[] vector)
	{
		var point = cardEdition.ToPointStruct(vector);

		await _client.UpsertAsync(CollectionName, [point]);
	}

	public async Task RecreateCollectionAsync()
	{
		_logger.LogInformation("Recreating collection");
		await _client.RecreateCollectionAsync(CollectionName, CreateVectorParams(), hnswConfig: CreateHnswConfig());
		_logger.LogInformation("Recreation completed");
	}

	/// <summary>
	/// Initializes the Qdrant service and makes sure the collection exists.
	/// Should be called at startup.
	/// </summary>
	public async Task InitializeAsync()
	{
		if (!await _client.CollectionExistsAsync(CollectionName))
		{
			await _client.CreateCollectionAsync(CollectionName, CreateVectorParams(), hnswConfig: CreateHnswConfig());
		}
	}

	public enum SearchQuality
	{
		Dev = 64,
		Normal = 128,
		High = 256,
	}

	private static VectorParams CreateVectorParams()
	{
		return new VectorParams()
		{
			Size = VectorSize,
			Distance = VectorDistance,
		};
	}

	private static HnswConfigDiff CreateHnswConfig()
	{
		return new HnswConfigDiff()
		{
			M = 40,
			EfConstruct = 512
		};
	}

	private QdrantClient CreateClient()
	{
		_logger.LogDebug("Creating Qdrant client with {Host}:{Port}", Host, Port);
		return new QdrantClient(Host, Port, https: false);
	}
}

public static class ScryfallCardObjectExtensions
{
	public static PointStruct ToPointStruct(this CardEdition cardEdition, float[] vectors)
	{
		var payload = new CardPayload(cardEdition);
		return new PointStruct()
		{
			Id = cardEdition.CardId,
			Vectors = vectors,
			Payload =
			{
				payload.ToMap()
			}
		};
	}
}