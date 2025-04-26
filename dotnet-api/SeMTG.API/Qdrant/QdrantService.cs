using System.Text.Json;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using SeMTG.API.Features.ScryfallImport;
using SeMTG.API.Models;

namespace SeMTG.API.Qdrant;

public class QdrantService
{
	private readonly string _collectionName = "mtg-cards";
	private readonly string _host = "localhost";
	private readonly int _port = 6334;
	private readonly ulong _vectorSize = 384;
	private readonly Distance _vectorDistance = Distance.Cosine;
	private readonly ILogger<QdrantService> _logger;
	private readonly QdrantClient _client;

	public QdrantService(ILogger<QdrantService> logger)
	{
		_logger = logger;
		_client = CreateClient();
	}

	public async Task<List<CardPayload>> SearchAsync(float[] vector,
		ulong limit)
	{
		var result = await _client.SearchAsync(_collectionName, vector, limit: limit);

		return result.Select(hit => new CardPayload(hit)).ToList();
	}

	public async Task UpsertCardAsync(ScryfallCardObject card,
		float[] vector)
	{
		var payload = new CardPayload(card);
		var id = GenerateId(card);

		var point = new PointStruct()
		{
			Id = id,
			Vectors = vector,
			Payload =
			{
				payload.ToMap()
			}
		};

		await _client.UpsertAsync(_collectionName, [point]);
	}

	/// <summary>
	/// Initializes the Qdrant service and makes sure the collection exists.
	/// Should be called at startup.
	/// </summary>
	public async Task InitializeAsync()
	{
		if (!await _client.CollectionExistsAsync(_collectionName))
		{
			await _client.CreateCollectionAsync(_collectionName, new VectorParams()
			{
				Size = _vectorSize,
				Distance = _vectorDistance,
			});
		}
	}

	private QdrantClient CreateClient()
	{
		_logger.LogDebug("Creating Qdrant client with {Host}:{Port}", _host, _port);
		return new QdrantClient(_host, _port, https: false);
	}

	public record CardPayload(string Name, string TypeLine, string OracleText)
	{
		public CardPayload(ScryfallCardObject card) : this(card.Name, card.TypeLine, card.OracleText)
		{
		}

		public CardPayload(ScoredPoint scoredPoint)
			: this(scoredPoint.Payload[nameof(Name)].StringValue, scoredPoint.Payload[nameof(TypeLine)].StringValue, scoredPoint.Payload[nameof(OracleText)].StringValue)
		{
		}

		public IDictionary<string, Value> ToMap()
		{
			var map = new Dictionary<string, Value>
			{
				{ nameof(Name), Name },
				{ nameof(TypeLine), TypeLine },
				{ nameof(OracleText), OracleText }
			};
			return map;
		}
	}

	private static ulong GenerateId(ScryfallCardObject card)
	{
		return HashStringToUlong(card.Name);
	}

	/// <summary>
	/// Mainly used to hash the card name to a ulong for Qdrant's Id field.
	/// </summary>
	// FNV-1a 64-bit string hash
	private static ulong HashStringToUlong(string input)
	{
		const ulong fnvOffsetBasis = 14695981039346656037;
		const ulong fnvPrime = 1099511628211;

		ulong hash = fnvOffsetBasis;
		foreach (var c in input)
		{
			hash ^= c;
			hash *= fnvPrime;
		}

		return hash;
	}
}