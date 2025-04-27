using Qdrant.Client.Grpc;
using SeMTG.API.Models;

namespace SeMTG.API.Qdrant;

public record CardPayload(string Name, string TypeLine, string OracleText)
{
	public CardPayload(CardEdition card) : this(card.Name, card.TypeLine, card.OracleText)
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