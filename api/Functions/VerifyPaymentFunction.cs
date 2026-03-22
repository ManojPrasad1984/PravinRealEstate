using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using LuckyDraw.Api.Data;
using LuckyDraw.Api.Helpers;
using LuckyDraw.Api.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace LuckyDraw.Api.Functions;

public class VerifyPaymentFunction
{
    private readonly AppDbContext _dbContext;

    public VerifyPaymentFunction(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Function("VerifyPayment")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "payments/verify")] HttpRequestData req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var payload = JsonSerializer.Deserialize<PaymentVerifyRequest>(requestBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (payload?.Entry is null)
        {
            return await CreateError(req, "Invalid request payload.");
        }

        var entry = payload.Entry;

        if (string.IsNullOrWhiteSpace(entry.FullName))
            return await CreateError(req, "Full Name is required.");

        if (!Regex.IsMatch(entry.MobileNumber ?? string.Empty, "^[6-9][0-9]{9}$"))
            return await CreateError(req, "Invalid mobile number.");

        if (!Regex.IsMatch(entry.AadhaarNumber ?? string.Empty, "^[0-9]{12}$"))
            return await CreateError(req, "Invalid Aadhaar number.");

        var paymentExists = await _dbContext.LuckyDrawEntries
            .AnyAsync(x => x.PaymentId == payload.RazorpayPaymentId);

        if (paymentExists)
            return await CreateError(req, "Payment already processed.");

        var secret = Environment.GetEnvironmentVariable("RazorpaySecret") ?? string.Empty;
        var expectedSignature = PaymentHelper.GenerateSignature(payload.RazorpayOrderId, payload.RazorpayPaymentId, secret);

        if (!string.Equals(expectedSignature, payload.RazorpaySignature, StringComparison.OrdinalIgnoreCase))
            return await CreateError(req, "Payment verification failed.");

        entry.PaymentId = payload.RazorpayPaymentId;
        entry.RazorpayOrderId = payload.RazorpayOrderId;
        entry.PaymentStatus = true;
        entry.EntryDate = DateTime.UtcNow;
        entry.EntryAmount = decimal.TryParse(Environment.GetEnvironmentVariable("EntryAmount"), out var amount) ? amount : 500;
        entry.PrizeChoice = string.IsNullOrWhiteSpace(entry.PrizeChoice) ? "No Prize" : entry.PrizeChoice;
        entry.AgentId = entry.AgentId == Guid.Empty ? null : entry.AgentId;
        entry.CardNumber = await GenerateCardNumber();

        _dbContext.LuckyDrawEntries.Add(entry);
        await _dbContext.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync(JsonSerializer.Serialize(new
        {
            success = true,
            cardNumber = entry.CardNumber
        }));
        response.Headers.Add("Content-Type", "application/json");

        return response;
    }

    private async Task<string> GenerateCardNumber()
    {
        var lastId = await _dbContext.LuckyDrawEntries
            .OrderByDescending(x => x.Id)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        var next = lastId + 1;
        return $"WL-{DateTime.UtcNow.Year}-{next:D5}";
    }

    private static async Task<HttpResponseData> CreateError(HttpRequestData req, string message)
    {
        var response = req.CreateResponse(HttpStatusCode.BadRequest);
        await response.WriteStringAsync(JsonSerializer.Serialize(new { success = false, message }));
        response.Headers.Add("Content-Type", "application/json");
        return response;
    }
}
