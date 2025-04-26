using System.Text.Json.Serialization;

namespace SeMTG.API.Models;

public class ImageUris
{
	public Guid Id { get; set; }
	public Guid ScryfallCardObjectId { get; set; }
	[JsonPropertyName("small")] public string Small { get; set; }

	[JsonPropertyName("normal")] public string Normal { get; set; }

	[JsonPropertyName("large")] public string Large { get; set; }

	[JsonPropertyName("png")] public string Png { get; set; }

	[JsonPropertyName("art_crop")] public string ArtCrop { get; set; }

	[JsonPropertyName("border_crop")] public string BorderCrop { get; set; }
}