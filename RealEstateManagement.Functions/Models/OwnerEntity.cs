using System.ComponentModel.DataAnnotations;

namespace RealEstateManagement.Functions.Models;

public class OwnerEntity
{
    [Key]
    public int OwnerId { get; set; }

    public string OwnerName { get; set; } = string.Empty;

    public string Own_ContactInformation { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    public int? TenantId { get; set; }

    public int? AssetId { get; set; }
}
