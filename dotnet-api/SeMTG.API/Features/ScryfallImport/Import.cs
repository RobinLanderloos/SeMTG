using Microsoft.AspNetCore.Http.HttpResults;
using SeMTG.API.ScryfallImport;

namespace SeMTG.API.Features.ScryfallImport;

public static class Import
{
	public static void MapImport(this IEndpointRouteBuilder builder)
	{
		builder.MapPost("/trigger-import", Ok<Guid> (ScryfallCardsImporter scryFallCardsImporter, string path = @"F:\Workspace\.NET\SeMTG\data\default-cards.json") =>
		{
			// Should be used later to track the import in some way
			var importGuid = Guid.NewGuid();

			_ = scryFallCardsImporter.Import(path);

			return TypedResults.Ok(importGuid);
		});
	}
}