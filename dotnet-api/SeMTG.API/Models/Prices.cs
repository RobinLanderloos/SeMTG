using System.Text.Json.Serialization;

namespace SeMTG.API.Models;

public class Prices
{
	public Guid CardEditionId { get; set; }
	public CardEdition CardEdition { get; set; } = null!;
	[JsonPropertyName("usd")] public string? Usd { get; set; }

	[JsonPropertyName("usd_foil")] public string? UsdFoil { get; set; }

	[JsonPropertyName("usd_etched")] public string? UsdEtched { get; set; }

	[JsonPropertyName("eur")] public string? Eur { get; set; }

	[JsonPropertyName("eur_foil")] public string? EurFoil { get; set; }

	[JsonPropertyName("tix")] public string? Tix { get; set; }
}