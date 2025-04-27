using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;

namespace SeMTG.API.Features.Admin;

public static class UpdateDatabase
{
	public static void MapUpdateDatabase(this IEndpointRouteBuilder builder)
	{
		builder.MapPost("/update-database", UpdateDatabaseAsync);
	}

	public static async Task<Results<Ok, InternalServerError<string>>> UpdateDatabaseAsync(ApplicationDbContext dbContext)
	{
		try
		{
			await dbContext.Database.MigrateAsync();
			return TypedResults.Ok();
		}
		catch (Exception ex)
		{
			return TypedResults.InternalServerError(ex.Message);
		}
	}
}