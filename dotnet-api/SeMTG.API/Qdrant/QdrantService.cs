using Qdrant.Client;
using Qdrant.Client.Grpc;
using SeMTG.API.Models;

namespace SeMTG.API.Qdrant;

public class QdrantService
{
	private const string CollectionName = "mtg-cards";
	private readonly string _host;
	private readonly int _port;
	private const ulong VectorSize = 768;
	private const Distance VectorDistance = Distance.Cosine;

	private readonly ILogger<QdrantService> _logger;
	private readonly QdrantClient _client;

	public QdrantService(ILogger<QdrantService> logger,
		IConfiguration configuration)
	{
		_logger = logger;
		_port = int.Parse(configuration["Qdrant:Port"] ?? throw new InvalidOperationException("Qdrant:Port is not set"));
		_host = configuration["Qdrant:Host"] ?? throw new InvalidOperationException("Qdrant:Host is not set");

		_client = CreateClient();
	}

	public async Task<List<ScoredPoint>> SearchAsync(float[] vector,
		ulong limit,
		SearchQuality searchQuality = SearchQuality.Normal)
	{
		var result = await _client.SearchAsync(CollectionName, vector, limit: limit, searchParams: new SearchParams()
		{
			HnswEf = (ulong)searchQuality
		});

		return result.ToList();
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

	public async Task UpdatePayloadAsync(CardEdition cardEdition)
	{
		var payload = new CardPayload(cardEdition).ToMapFieldDictionary();

		await _client.OverwritePayloadAsync(CollectionName, payload, cardEdition.GetPointId());
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
		_logger.LogInformation("Creating Qdrant client with {Host}:{Port}", _host, _port);
		return new QdrantClient(_host, _port, https: false);
	}
}

internal static class CardEditionExtensions
{
	public static PointStruct ToPointStruct(this CardEdition cardEdition,
		float[] vectors)
	{
		var payload = new CardPayload(cardEdition);
		var id = cardEdition.GetPointId();
		return new PointStruct()
		{
			Id = id,
			Vectors = vectors,
			Payload =
			{
				payload.ToMapFieldDictionary()
			}
		};
	}

	public static Guid GetPointId(this CardEdition cardEdition)
	{
		return cardEdition.CardId;
	}
}