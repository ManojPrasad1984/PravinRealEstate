using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")] HttpRequestData req)
    {
        var razorpayKey = Environment.GetEnvironmentVariable("RazorpayKey");
        var razorpaySecret = Environment.GetEnvironmentVariable("RazorpaySecret");
        var entryAmount = int.TryParse(Environment.GetEnvironmentVariable("EntryAmount"), out var amount) ? amount : 500;

        if (string.IsNullOrWhiteSpace(razorpayKey) || string.IsNullOrWhiteSpace(razorpaySecret))
        {
            var badConfig = req.CreateResponse(HttpStatusCode.InternalServerError);
            await badConfig.WriteStringAsync("Razorpay configuration missing.");
            return badConfig;
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

        var response = req.CreateResponse(result.IsSuccessStatusCode ? HttpStatusCode.OK : HttpStatusCode.BadRequest);

        if (!result.IsSuccessStatusCode)
        {
            await response.WriteStringAsync(resultBody);
            return response;
        }

        using var jsonDoc = JsonDocument.Parse(resultBody);
        var orderId = jsonDoc.RootElement.GetProperty("id").GetString();

        await response.WriteStringAsync(JsonSerializer.Serialize(new
        {
            orderId,
            amount = entryAmount,
            key = razorpayKey
        }));

        response.Headers.Add("Content-Type", "application/json");
        return response;
    }
}
