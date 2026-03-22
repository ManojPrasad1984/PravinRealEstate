using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace LuckyDraw.Api.Helpers;

public static class HttpResponseHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static HttpResponseData CreateCorsResponse(HttpRequestData req, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = req.CreateResponse(statusCode);
        AddCorsHeaders(response);
        return response;
    }

    public static async Task<HttpResponseData> CreateJsonResponse(HttpRequestData req, object payload, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = CreateCorsResponse(req, statusCode);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(payload, JsonOptions));
        return response;
    }

    public static void AddCorsHeaders(HttpResponseData response)
    {
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type,Authorization");
    }
}
