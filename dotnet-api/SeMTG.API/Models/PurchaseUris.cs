using System.Text.Json.Serialization;

namespace SeMTG.API.Models;

public class PurchaseUris
{
	public Guid CardEditionId { get; set; }
	public CardEdition CardEdition { get; set; } = null!;
	[JsonPropertyName("tcgplayer")] public string Tcgplayer { get; set; }

	[JsonPropertyName("cardmarket")] public string Cardmarket { get; set; }

	[JsonPropertyName("cardhoarder")] public string Cardhoarder { get; set; }
}