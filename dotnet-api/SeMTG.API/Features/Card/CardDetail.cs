using System.Linq.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Features.Shared.Models;
using SeMTG.API.Models;

namespace SeMTG.API.Features.Card;

public class CardDetail
{
	private const string RouteTemplate = "/{cardId}";

	public static void MapCardDetail(IEndpointRouteBuilder builder)
	{
		builder.MapGet(RouteTemplate, GetCardDetailAsync);
	}

	private static async Task<Results<Ok<CardDetailResult>, NotFound>> GetCardDetailAsync(
		ILogger<CardDetail> logger,
		ApplicationDbContext dbContext,
		Guid cardId)
	{
		logger.LogInformation("Getting card detail for card {CardId}", cardId);

		var card = await dbContext
			.Cards
			.Include(c => c.Editions)
			.ThenInclude(e => e.ImageUris)
			.FirstOrDefaultAsync(c => c.Id == cardId);

		if (card == null)
		{
			return TypedResults.NotFound();
		}

		var result = new CardDetailResult(
			card.Id,
			card.Name,
			card.Editions.Select(e => new CardEditionDetail(
				e.Id,
				e.Name,
				e.TypeLine,
				e.OracleText,
				new ImageUrisDto(e.ImageUris)
			)).ToList()
		);

		return TypedResults.Ok(result);
	}
}

public record CardDetailResult(Guid Id, string Name, IReadOnlyCollection<CardEditionDetail> Editions);

public record CardEditionDetail(Guid Id, string Name, string TypeLine, string OracleText, ImageUrisDto ImageUris);