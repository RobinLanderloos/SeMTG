using System.Text.Json.Serialization;

namespace SeMTG.API.Models;

public class RelatedUris
{
	public Guid CardEditionId { get; set; }
	public CardEdition CardEdition { get; set; } = null!;
	[JsonPropertyName("gatherer")] public string? Gatherer { get; set; }

	[JsonPropertyName("tcgplayer_infinite_articles")]
	public string? TcgplayerInfiniteArticles { get; set; }

	[JsonPropertyName("tcgplayer_infinite_decks")]
	public string? TcgplayerInfiniteDecks { get; set; }

	[JsonPropertyName("edhrec")] public string? Edhrec { get; set; }
}