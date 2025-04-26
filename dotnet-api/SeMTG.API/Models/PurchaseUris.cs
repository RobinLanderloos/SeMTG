using System.Text.Json.Serialization;

namespace SeMTG.API.Models;

public class PurchaseUris
{
	public Guid Id { get; set; }
	public Guid ScryfallCardObjectId { get; set; }
	[JsonPropertyName("tcgplayer")] public string Tcgplayer { get; set; }

	[JsonPropertyName("cardmarket")] public string Cardmarket { get; set; }

	[JsonPropertyName("cardhoarder")] public string Cardhoarder { get; set; }
}