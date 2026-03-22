using LuckyDraw.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LuckyDraw.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<LuckyDrawEntry> LuckyDrawEntries => Set<LuckyDrawEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agent>().ToTable("Agents", "Wealthline_LuckyDraw");
        modelBuilder.Entity<LuckyDrawEntry>().ToTable("LuckyDrawEntries", "dbo");
    }
}
