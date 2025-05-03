using Google.Protobuf.Collections;
using Qdrant.Client.Grpc;
using SeMTG.API.Models;

namespace SeMTG.API.Qdrant;

public record CardPayload(string Name, string TypeLine, string OracleText, double Cmc, IReadOnlyCollection<string> Colors, IReadOnlyCollection<string> ColorIdentity)
{
	public CardPayload(CardEdition cardEdition) : this(cardEdition.Name, cardEdition.TypeLine, cardEdition.OracleText, cardEdition.Cmc, cardEdition.Colors, cardEdition.ColorIdentity)
	{
	}

	public CardPayload(ScoredPoint scoredPoint)
		: this(scoredPoint.Payload[nameof(Name)].StringValue,
			scoredPoint.Payload[nameof(TypeLine)].StringValue,
			scoredPoint.Payload[nameof(OracleText)].StringValue,
			scoredPoint.Payload[nameof(Cmc)].DoubleValue,
			scoredPoint.Payload[nameof(Colors)].ListValue.Values.Select(x => x.StringValue).ToList(),
			scoredPoint.Payload[nameof(ColorIdentity)].ListValue.Values.Select(x => x.StringValue).ToList())
	{
	}

	public MapField<string, Value> ToMapFieldDictionary()
	{
		var map = new MapField<string, Value>
		{
			{ nameof(Name), Name },
			{ nameof(TypeLine), TypeLine },
			{ nameof(OracleText), OracleText },
			{ nameof(Colors), Colors.ToArray() },
			{ nameof(ColorIdentity), ColorIdentity.ToArray() },
			{ nameof(Cmc), Cmc }
		};
		return map;
	}
}
