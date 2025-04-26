namespace SeMTG.API.Models;

public class QdrantInfo
{
	public Guid Id { get; set; }
	public Guid ScryfallCardObjectId { get; set; }
	public float[] Vector { get; set; }

	public QdrantInfo(Guid scryfallCardObjectId,
		float[] vector)
	{
		ScryfallCardObjectId = scryfallCardObjectId;
		Vector = vector;
	}

#pragma warning disable CS8618, CS9264
	private QdrantInfo()
#pragma warning restore CS8618, CS9264
	{
	}
}