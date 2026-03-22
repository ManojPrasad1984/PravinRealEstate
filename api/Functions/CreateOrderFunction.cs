using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LuckyDraw.Api.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LuckyDraw.Api.Functions;

public class CreateOrderFunction
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateOrderFunction(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [Function("CreateOrder")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "orders")] HttpRequestData req)
    {
        if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            return HttpResponseHelper.CreateCorsResponse(req, HttpStatusCode.NoContent);
        }

        var razorpayKey = Environment.GetEnvironmentVariable("RazorpayKey");
        var razorpaySecret = Environment.GetEnvironmentVariable("RazorpaySecret");
        var entryAmount = int.TryParse(Environment.GetEnvironmentVariable("EntryAmount"), out var amount) ? amount : 500;

        if (string.IsNullOrWhiteSpace(razorpayKey) || string.IsNullOrWhiteSpace(razorpaySecret))
        {
            return await HttpResponseHelper.CreateJsonResponse(req,
                new { success = false, message = "Razorpay configuration missing." },
                HttpStatusCode.InternalServerError);
        }

        var client = _httpClientFactory.CreateClient();
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{razorpayKey}:{razorpaySecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var body = JsonSerializer.Serialize(new
        {
            amount = entryAmount * 100,
            currency = "INR",
            receipt = $"LuckyDraw_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"
        });

        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var result = await client.PostAsync("https://api.razorpay.com/v1/orders", content);
        var resultBody = await result.Content.ReadAsStringAsync();

        if (!result.IsSuccessStatusCode)
        {
            return await HttpResponseHelper.CreateJsonResponse(req,
                new { success = false, message = "Failed to create Razorpay order.", detail = resultBody },
                HttpStatusCode.BadRequest);
        }

        using var jsonDoc = JsonDocument.Parse(resultBody);
        var orderId = jsonDoc.RootElement.GetProperty("id").GetString();

        return await HttpResponseHelper.CreateJsonResponse(req, new
        {
            orderId,
            amount = entryAmount,
            key = razorpayKey
        });
    }
}
