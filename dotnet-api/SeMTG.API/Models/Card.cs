namespace SeMTG.API.Models;

public class Card
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public float[]? Vector { get; private set; }
	public List<CardEdition> Editions { get; private set; } = new();

	public Card(Guid id, string name)
	{
		Id = id;
		Name = name;
	}

	public void AddEdition(CardEdition edition)
	{
		Editions.Add(edition);
	}

	public void SetVector(float[] vector)
	{
		Vector = vector;
	}

	public CardEdition? GetLatestEdition()
	{
		return Editions.OrderByDescending(edition => edition.ReleasedAt).FirstOrDefault();
	}
}