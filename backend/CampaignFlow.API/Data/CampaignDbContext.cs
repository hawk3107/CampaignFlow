using CampaignFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CampaignFlow.API.Data;

public class CampaignDbContext : DbContext
{
    public CampaignDbContext(DbContextOptions<CampaignDbContext> options)
        : base(options)
    {
    }

    public DbSet<Campaign> Campaigns { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    modelBuilder.Entity<Campaign>()
        .Property(c => c.Budget)
        .HasPrecision(18, 2);

    modelBuilder.Entity<Campaign>()
        .Property(c => c.Spend)
        .HasPrecision(18, 2);
    }
}