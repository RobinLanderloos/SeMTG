using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SeMTG.API.Database;
using SeMTG.API.Models;

namespace SeMTG.API.ScryfallImport;
public class ScryfallCardsImporter(ILogger<ScryfallCardsImporter> logger, IServiceScopeFactory serviceScopeFactory)
{
    private const int BatchSize = 128;

    public async Task Import(string path)
    {
        try
        {
            logger.LogInformation("Starting import for path {Path}", path);

            var cards = await LoadAndFilterCardsAsync(path);

            var chunks = SplitIntoChunks(cards);

            await ProcessChunksAsync(chunks);

            logger.LogInformation("Import completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Import failed.");
        }
    }

    private async Task<List<CardEdition>> LoadAndFilterCardsAsync(string path)
    {
        logger.LogInformation("Loading cards from {Path}", path);

        var cards = await LoadCardsAsync(path);

        logger.LogDebug("Loaded {Count} cards", cards.Count);

        var filteredCards = cards
            .Where(card =>
                !string.IsNullOrEmpty(card.OracleText) &&
                !(card.TypeLine?.Contains("token", StringComparison.OrdinalIgnoreCase) ?? false) &&
                card.Lang == "en")
            .ToList();

        logger.LogInformation("Filtered down to {Count} cards", filteredCards.Count);

        return filteredCards;
    }

    private List<CardEdition[]> SplitIntoChunks(List<CardEdition> cards)
    {
        var chunks = cards.Chunk(BatchSize).ToList();

        logger.LogInformation("Split into {Count} chunks (BatchSize={BatchSize})", chunks.Count, BatchSize);

        return chunks;
    }

    private async Task ProcessChunksAsync(List<CardEdition[]> chunks)
    {
        for (var i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];

            logger.LogInformation("Processing chunk {ChunkIndex}/{ChunkCount} with {CardCount} cards", i, chunks.Count, chunk.Length);

            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await AddCardsToDatabaseAsync(dbContext, chunk);

            logger.LogInformation("Finished processing chunk {ChunkIndex}/{ChunkCount}", i, chunks.Count);
        }
    }

    private async Task AddCardsToDatabaseAsync(ApplicationDbContext dbContext, CardEdition[] cardEditions)
    {
        foreach (var cardEdition in cardEditions)
        {
            var existingEdition = await dbContext.CardEditions.FirstOrDefaultAsync(x => x.Id == cardEdition.Id);
            if (existingEdition != null)
            {
                logger.LogDebug("Card edition {CardName} already exists, skipping", cardEdition.Name);
                continue;
            }

            logger.LogDebug("Adding card edition {CardName}", cardEdition.Name);
            var card = await dbContext.Cards.FirstOrDefaultAsync(x => x.Name == cardEdition.Name);

            if (card == null)
            {
                logger.LogDebug("Card {CardName} not found, creating new card", cardEdition.Name);
                card = new Card(Guid.NewGuid(), cardEdition.Name);
                dbContext.Cards.Add(card);
            }

            logger.LogDebug("Adding card edition {CardName} to card {CardId}", cardEdition.Name, card.Id);
            dbContext.CardEditions.Add(cardEdition);
            card.AddEdition(cardEdition);
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task<List<CardEdition>> LoadCardsAsync(string path)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        await using var fs = File.OpenRead(path);
        var cards = await JsonSerializer.DeserializeAsync<List<CardEdition>>(fs, options);

        if (cards == null) throw new ArgumentException("Failed to deserialize cards");

        return cards;
    }
}
