namespace LuckyDraw.Api.Models;

public class LuckyDrawEntry
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string AadhaarNumber { get; set; } = string.Empty;
    public string? PrizeChoice { get; set; }
    public decimal? EntryAmount { get; set; }
    public string? RazorpayOrderId { get; set; }
    public string? PaymentId { get; set; }
    public bool? PaymentStatus { get; set; }
    public string? CardNumber { get; set; }
    public DateTime? EntryDate { get; set; }
    public Guid? AgentId { get; set; }

    public Agent? Agent { get; set; }
}
