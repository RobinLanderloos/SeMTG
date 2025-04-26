using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Embedding;
using SeMTG.API.Features.Admin;
using SeMTG.API.Features.Query;
using SeMTG.API.Features.ScryfallImport;
using SeMTG.API.Qdrant;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddSingleton<QdrantService>();
builder.Services.AddScoped<IEmbeddingService, PythonHttpEmbeddingService>();
builder.Services.AddScoped<ScryfallCardsImporter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();

	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/openapi/v1.json", "SeMTG API V1");
	});
}

using var scope = app.Services.CreateScope();
var qdrantService = scope.ServiceProvider.GetRequiredService<QdrantService>();

await qdrantService.InitializeAsync();

// Admin
app.MapRecreateCollection();
app.MapUpdateDatabase();

app.MapImport();
app.MapQuery();

app.UseHttpsRedirection();

await app.RunAsync();