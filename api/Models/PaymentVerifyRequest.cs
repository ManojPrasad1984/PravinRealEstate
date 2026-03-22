using System.Text.Json.Serialization;

namespace LuckyDraw.Api.Models;

public class PaymentVerifyRequest
{
    public LuckyDrawEntry Entry { get; set; } = new();

    [JsonPropertyName("razorpay_payment_id")]
    public string RazorpayPaymentId { get; set; } = string.Empty;

    [JsonPropertyName("razorpay_order_id")]
    public string RazorpayOrderId { get; set; } = string.Empty;

    [JsonPropertyName("razorpay_signature")]
    public string RazorpaySignature { get; set; } = string.Empty;
}
