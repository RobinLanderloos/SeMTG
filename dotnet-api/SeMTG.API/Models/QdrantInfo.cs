namespace SeMTG.API.Models;

public class QdrantInfo
{
	public Guid Id { get; set; }
	public Guid ScryfallCardObjectId { get; set; }
	public float[] Vector { get; set; }
}