using Qdrant.Client;
using Qdrant.Client.Grpc;
using SeMTG.API.Models;

namespace SeMTG.API.Qdrant;

public class QdrantService
{
	private const string CollectionName = "mtg-cards";
	private const string Host = "localhost";
	private const int Port = 6334;
	private const ulong VectorSize = 384;
	private const Distance VectorDistance = Distance.Cosine;

	private readonly ILogger<QdrantService> _logger;
	private readonly ICardPayloadIdGenerator _cardPayloadIdGenerator;
	private readonly QdrantClient _client;

	public QdrantService(ILogger<QdrantService> logger,
		ICardPayloadIdGenerator cardPayloadIdGenerator)
	{
		_logger = logger;
		_cardPayloadIdGenerator = cardPayloadIdGenerator;
		_client = CreateClient();
	}

	public async Task<List<CardPayload>> SearchAsync(float[] vector,
		ulong limit)
	{
		var result = await _client.SearchAsync(CollectionName, vector, limit: limit);

		return result.Select(hit => new CardPayload(hit)).ToList();
	}

	public async Task UpsertCardAsync(ScryfallCardObject card,
		float[] vector)
	{
		var payload = new CardPayload(card);
		var id = _cardPayloadIdGenerator.GenerateId(card);

		var point = new PointStruct()
		{
			Id = id,
			Vectors = vector,
			Payload =
			{
				payload.ToMap()
			}
		};

		await _client.UpsertAsync(CollectionName, [point]);
	}

	/// <summary>
	/// Initializes the Qdrant service and makes sure the collection exists.
	/// Should be called at startup.
	/// </summary>
	public async Task InitializeAsync()
	{
		if (!await _client.CollectionExistsAsync(CollectionName))
		{
			await _client.CreateCollectionAsync(CollectionName, new VectorParams()
			{
				Size = VectorSize,
				Distance = VectorDistance,
			});
		}
	}

	private QdrantClient CreateClient()
	{
		_logger.LogDebug("Creating Qdrant client with {Host}:{Port}", Host, Port);
		return new QdrantClient(Host, Port, https: false);
	}
}