using Microsoft.EntityFrameworkCore;
using SeMTG.API.Models;

namespace SeMTG.API.Database;

public class ApplicationDbContext : DbContext
{
	public DbSet<ScryfallCardObject> ScryfallCards { get; set; }
	public DbSet<RelatedUris> RelatedUris { get; set; }
	public DbSet<ImageUris> ImageUris { get; set; }
	public DbSet<Legalities> Legalities { get; set; }
	public DbSet<Prices> Prices { get; set; }
	public DbSet<PurchaseUris> PurchaseUris { get; set; }
	public DbSet<QdrantInfo> QdrantInfo { get; set; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ScryfallCardObject>().HasIndex(card => card.Name);
		modelBuilder.Entity<ScryfallCardObject>().HasIndex(card => card.TypeLine);
		modelBuilder.Entity<ScryfallCardObject>().HasIndex(card => card.OracleText);
	}
}