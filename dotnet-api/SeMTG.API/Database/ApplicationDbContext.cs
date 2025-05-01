using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SeMTG.API.Models;

namespace SeMTG.API.Database;

public sealed class ApplicationDbContext : DbContext
{
	public DbSet<Card> Cards { get; set; }
	public DbSet<CardEdition> CardEditions { get; set; }
	public DbSet<RelatedUris> RelatedUris { get; set; }
	public DbSet<ImageUris> ImageUris { get; set; }
	public DbSet<Legalities> Legalities { get; set; }
	public DbSet<Prices> Prices { get; set; }
	public DbSet<PurchaseUris> PurchaseUris { get; set; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
		ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ConfigureCard(modelBuilder);
		ConfigureCardEdition(modelBuilder);
		ConfigureImageUris(modelBuilder);
		ConfigureLegalities(modelBuilder);
		ConfigurePrices(modelBuilder);
		ConfigurePurchaseUris(modelBuilder);
		ConfigureRelatedUris(modelBuilder);
	}

	private static void ConfigureCard(ModelBuilder modelBuilder)
	{
		var cardBuilder = modelBuilder.Entity<Card>();
		cardBuilder.HasIndex(card => card.Name);

		cardBuilder.HasMany<CardEdition>(card => card.Editions).WithOne(edition => edition.Card).HasForeignKey(edition => edition.CardId);
	}

	private static void ConfigureCardEdition(ModelBuilder modelBuilder)
	{
		var cardEditionBuilder = modelBuilder.Entity<CardEdition>();
		cardEditionBuilder.HasIndex(card => card.Name);
		cardEditionBuilder.HasIndex(card => card.CardId);
		cardEditionBuilder.HasIndex(card => card.TypeLine);
		cardEditionBuilder.HasIndex(card => card.OracleText);

		cardEditionBuilder.HasOne<ImageUris>(c => c.ImageUris).WithOne(uri => uri.CardEdition).HasForeignKey<ImageUris>(uri => uri.CardEditionId);
		cardEditionBuilder.HasOne<Legalities>(c => c.Legalities).WithOne(legality => legality.CardEdition).HasForeignKey<Legalities>(legality => legality.CardEditionId);
		cardEditionBuilder.HasOne<Prices>(c => c.Prices).WithOne(price => price.CardEdition).HasForeignKey<Prices>(price => price.CardEditionId);
		cardEditionBuilder.HasOne<PurchaseUris>(c => c.PurchaseUris).WithOne(purchase => purchase.CardEdition).HasForeignKey<PurchaseUris>(purchase => purchase.CardEditionId);
		cardEditionBuilder.HasOne<RelatedUris>(c => c.RelatedUris).WithOne(related => related.CardEdition).HasForeignKey<RelatedUris>(related => related.CardEditionId);
	}

	private static void ConfigureImageUris(ModelBuilder modelBuilder)
	{
		var imageUrisBuilder = modelBuilder.Entity<ImageUris>();
		imageUrisBuilder.HasKey(uri => uri.CardEditionId);
	}

	private static void ConfigureLegalities(ModelBuilder modelBuilder)
	{
		var legalitiesBuilder = modelBuilder.Entity<Legalities>();
		legalitiesBuilder.HasKey(legality => legality.CardEditionId);
	}

	private static void ConfigurePrices(ModelBuilder modelBuilder)
	{
		var pricesBuilder = modelBuilder.Entity<Prices>();
		pricesBuilder.HasKey(price => price.CardEditionId);
	}

	private static void ConfigurePurchaseUris(ModelBuilder modelBuilder)
	{
		var purchaseUrisBuilder = modelBuilder.Entity<PurchaseUris>();
		purchaseUrisBuilder.HasKey(purchase => purchase.CardEditionId);
	}

	private static void ConfigureRelatedUris(ModelBuilder modelBuilder)
	{
		var relatedUrisBuilder = modelBuilder.Entity<RelatedUris>();
		relatedUrisBuilder.HasKey(related => related.CardEditionId);
	}
}