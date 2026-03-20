
using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Models;
using RealEstateManagement.Services;
namespace RealEstateManagement.Data
{
  

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LuckyDrawEntry> LuckyDrawEntries { get; set; }
        public DbSet<Agent> Agents { get; set; }
    }
}
