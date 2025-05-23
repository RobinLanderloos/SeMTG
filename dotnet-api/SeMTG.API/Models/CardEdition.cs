﻿using System.Text.Json.Serialization;

namespace SeMTG.API.Models;

#nullable disable

public class CardEdition
{
	public Guid CardId { get; set; }
	public Card Card { get; set; } = null!;

	[JsonPropertyName("id")] public Guid Id { get; set; }

	[JsonPropertyName("object")] public string Object { get; set; }

	[JsonPropertyName("oracle_id")] public string OracleId { get; set; }

	[JsonPropertyName("multiverse_ids")] public List<int?> MultiverseIds { get; set; }

	[JsonPropertyName("mtgo_id")] public int? MtgoId { get; set; }

	[JsonPropertyName("arena_id")] public int? ArenaId { get; set; }

	[JsonPropertyName("tcgplayer_id")] public int? TcgplayerId { get; set; }

	[JsonPropertyName("name")] public string Name { get; set; }

	[JsonPropertyName("lang")] public string Lang { get; set; }

	[JsonPropertyName("released_at")] public DateOnly ReleasedAt { get; set; }

	[JsonPropertyName("uri")] public string Uri { get; set; }

	[JsonPropertyName("scryfall_uri")] public string ScryfallUri { get; set; }

	[JsonPropertyName("layout")] public string Layout { get; set; }

	[JsonPropertyName("highres_image")] public bool? HighresImage { get; set; }

	[JsonPropertyName("image_status")] public string ImageStatus { get; set; }

	[JsonPropertyName("image_uris")] public ImageUris ImageUris { get; set; }

	[JsonPropertyName("mana_cost")] public string ManaCost { get; set; }

	[JsonPropertyName("cmc")] public double Cmc { get; set; }

	[JsonPropertyName("type_line")] public string TypeLine { get; set; }

	[JsonPropertyName("oracle_text")] public string OracleText { get; set; }

	[JsonPropertyName("colors")] public List<string> Colors { get; set; }

	[JsonPropertyName("color_identity")] public List<string> ColorIdentity { get; set; }

	[JsonPropertyName("keywords")] public List<string> Keywords { get; set; }

	[JsonPropertyName("produced_mana")] public List<string> ProducedMana { get; set; }

	[JsonPropertyName("legalities")] public Legalities Legalities { get; set; }

	[JsonPropertyName("games")] public List<string> Games { get; set; }

	[JsonPropertyName("reserved")] public bool? Reserved { get; set; }

	[JsonPropertyName("game_changer")] public bool? GameChanger { get; set; }

	[JsonPropertyName("foil")] public bool? Foil { get; set; }

	[JsonPropertyName("nonfoil")] public bool? Nonfoil { get; set; }

	[JsonPropertyName("finishes")] public List<string> Finishes { get; set; }

	[JsonPropertyName("oversized")] public bool? Oversized { get; set; }

	[JsonPropertyName("promo")] public bool? Promo { get; set; }

	[JsonPropertyName("reprint")] public bool? Reprint { get; set; }

	[JsonPropertyName("variation")] public bool? Variation { get; set; }

	[JsonPropertyName("set_id")] public string SetId { get; set; }

	[JsonPropertyName("set")] public string Set { get; set; }

	[JsonPropertyName("set_name")] public string SetName { get; set; }

	[JsonPropertyName("set_type")] public string SetType { get; set; }

	[JsonPropertyName("set_uri")] public string SetUri { get; set; }

	[JsonPropertyName("set_search_uri")] public string SetSearchUri { get; set; }

	[JsonPropertyName("scryfall_set_uri")] public string ScryfallSetUri { get; set; }

	[JsonPropertyName("rulings_uri")] public string RulingsUri { get; set; }

	[JsonPropertyName("prints_search_uri")]
	public string PrintsSearchUri { get; set; }

	[JsonPropertyName("collector_number")] public string CollectorNumber { get; set; }

	[JsonPropertyName("digital")] public bool? Digital { get; set; }

	[JsonPropertyName("rarity")] public string Rarity { get; set; }

	[JsonPropertyName("card_back_id")] public string CardBackId { get; set; }

	[JsonPropertyName("artist")] public string Artist { get; set; }

	[JsonPropertyName("artist_ids")] public List<string> ArtistIds { get; set; }

	[JsonPropertyName("illustration_id")] public string IllustrationId { get; set; }

	[JsonPropertyName("border_color")] public string BorderColor { get; set; }

	[JsonPropertyName("frame")] public string Frame { get; set; }

	[JsonPropertyName("full_art")] public bool? FullArt { get; set; }

	[JsonPropertyName("textless")] public bool? Textless { get; set; }

	[JsonPropertyName("booster")] public bool? Booster { get; set; }

	[JsonPropertyName("story_spotlight")] public bool? StorySpotlight { get; set; }

	[JsonPropertyName("prices")] public Prices Prices { get; set; }

	[JsonPropertyName("related_uris")] public RelatedUris RelatedUris { get; set; }

	[JsonPropertyName("purchase_uris")] public PurchaseUris PurchaseUris { get; set; }
}