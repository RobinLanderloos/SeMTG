using SeMTG.API.Models;

namespace SeMTG.API.Features.Shared.Models;

public record ImageUrisDto(string Small, string Normal, string Large, string Png, string ArtCrop, string BorderCrop)
{
	public ImageUrisDto(ImageUris imageUris) : this(imageUris.Small, imageUris.Normal, imageUris.Large, imageUris.Png, imageUris.ArtCrop, imageUris.BorderCrop)
	{
	}
}