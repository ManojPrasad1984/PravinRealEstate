using System.ComponentModel.DataAnnotations;

namespace RealEstateManagement.Functions.Models;

public class AssetEntity
{
    [Key]
    public int AssetId { get; set; }

    public string PropertyName { get; set; } = string.Empty;

    public string P_Address { get; set; } = string.Empty;

    public int NumberOfUnits { get; set; }

    public decimal RentAmount { get; set; }
}
