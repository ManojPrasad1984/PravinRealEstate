using System.Security.Cryptography;
using System.Text;

namespace LuckyDraw.Api.Helpers;

public static class PaymentHelper
{
    public static string GenerateSignature(string orderId, string paymentId, string secret)
    {
        var payload = $"{orderId}|{paymentId}";
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        return BitConverter.ToString(hmac.ComputeHash(payloadBytes)).Replace("-", "").ToLowerInvariant();
    }
}
