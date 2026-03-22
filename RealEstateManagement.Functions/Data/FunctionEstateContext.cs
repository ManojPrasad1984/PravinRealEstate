using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Functions.Models;

namespace RealEstateManagement.Functions.Data;

public class FunctionEstateContext(DbContextOptions<FunctionEstateContext> options) : DbContext(options)
{
    public DbSet<AssetEntity> AssetTble => Set<AssetEntity>();
    public DbSet<TenantEntity> TenantTble => Set<TenantEntity>();
    public DbSet<OwnerEntity> OwnerTble => Set<OwnerEntity>();
}
