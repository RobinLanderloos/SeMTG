using SeMTG.API.Models;

namespace SeMTG.API.Qdrant;

public interface ICardPayloadIdGenerator
{
	ulong GenerateId(ScryfallCardObject card);
}