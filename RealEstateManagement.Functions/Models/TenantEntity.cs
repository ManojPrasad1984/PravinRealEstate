using System.ComponentModel.DataAnnotations;

namespace RealEstateManagement.Functions.Models;

public class TenantEntity
{
    [Key]
    public int TenantId { get; set; }

    public string TenantName { get; set; } = string.Empty;

    public string T_ContactInformation { get; set; } = string.Empty;

    public DateTime LeaseStartDate { get; set; }

    public string? TenantImage { get; set; }
}
