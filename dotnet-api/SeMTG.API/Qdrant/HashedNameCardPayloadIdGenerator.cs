using SeMTG.API.Models;

namespace SeMTG.API.Qdrant;

public class HashedNameCardPayloadIdGenerator : ICardPayloadIdGenerator
{
	private readonly ILogger<HashedNameCardPayloadIdGenerator> _logger;

	public HashedNameCardPayloadIdGenerator(ILogger<HashedNameCardPayloadIdGenerator> logger)
	{
		_logger = logger;
	}

	public ulong GenerateId(ScryfallCardObject card)
	{
		_logger.LogDebug("Generating ID for card {CardName}", card.Name);
		// Maybe add some caching here to avoid hashing the same card multiple times

		return HashStringToUlong(card.Name);
	}

	/// <summary>
	/// Mainly used to hash the card name to an ulong for Quadrant's ID field.
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