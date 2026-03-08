
using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Models;
namespace RealEstateManagement.Data
{
  

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LuckyDrawEntry> LuckyDrawEntries { get; set; }
    }
}
