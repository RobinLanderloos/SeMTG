using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Models;

namespace SeMTG.API.Features.Admin;

public static class GetCardsWithMultipleDistinctOracleTexts
{
	public static void MapGetCardsWithMultipleDistinctOracleTexts(this IEndpointRouteBuilder builder)
	{
		builder.MapGet("/cards-with-multiple-distinct-oracle-texts", GetCardsWithMultipleDistinctOracleTextsAsync);
	}

	public static async Task<Ok<List<Card>>> GetCardsWithMultipleDistinctOracleTextsAsync(ApplicationDbContext dbContext)
	{
		var cards = await dbContext.Cards.Where(card => card.Editions.Select(ce => ce.OracleText).Distinct().Count() > 1).ToListAsync();

		return TypedResults.Ok(cards);
	}
}