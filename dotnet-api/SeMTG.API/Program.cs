using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Embedding;
using SeMTG.API.Features.Admin;
using SeMTG.API.Features.Embedding;
using SeMTG.API.Features.Query;
using SeMTG.API.Features.ScryfallImport;
using SeMTG.API.Qdrant;
using SeMTG.API.ScryfallImport;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddSingleton<QdrantService>();
builder.Services.AddScoped<IEmbeddingService, PythonHttpEmbeddingService>();
builder.Services.AddScoped<ScryfallCardsImporter>();
builder.Services.AddScoped<CardEmbedder>();

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
var adminGroup = app
	.MapGroup("/admin");
adminGroup.MapRecreateCollection();
adminGroup.MapUpdateDatabase();
adminGroup.MapGetCardsWithMultipleDistinctOracleTexts();

// Embedding
var embeddingGroup = app
	.MapGroup("/embedding");
embeddingGroup.MapGenerateEmbedding();
GenerateMissingEmbeddings.MapGenerateMissingEmbeddings(embeddingGroup);

// Import
var importGroup = app
	.MapGroup("/import");
importGroup.MapImport();

// Query
var queryGroup = app
	.MapGroup("/query");
queryGroup.MapQuery();

app.UseHttpsRedirection();

await app.RunAsync();