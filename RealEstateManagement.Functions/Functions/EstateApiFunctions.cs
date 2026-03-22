using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Functions.Data;
using RealEstateManagement.Functions.Models;

namespace RealEstateManagement.Functions.Functions;

public class EstateApiFunctions(FunctionEstateContext db)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Function("GetAssets")]
    public async Task<HttpResponseData> GetAssets(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assets")] HttpRequestData req)
    {
        var assets = await db.AssetTble.AsNoTracking().ToListAsync();
        return await CreateJsonResponse(req, HttpStatusCode.OK, assets);
    }

    [Function("CreateAsset")]
    public async Task<HttpResponseData> CreateAsset(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "assets")] HttpRequestData req)
    {
        var payload = await JsonSerializer.DeserializeAsync<AssetEntity>(req.Body, JsonOptions);
        if (payload is null || string.IsNullOrWhiteSpace(payload.PropertyName))
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new { message = "PropertyName is required." });
        }

        db.AssetTble.Add(payload);
        await db.SaveChangesAsync();
        return await CreateJsonResponse(req, HttpStatusCode.Created, payload);
    }

    [Function("GetTenants")]
    public async Task<HttpResponseData> GetTenants(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tenants")] HttpRequestData req)
    {
        var tenants = await db.TenantTble.AsNoTracking().ToListAsync();
        return await CreateJsonResponse(req, HttpStatusCode.OK, tenants);
    }

    [Function("CreateTenant")]
    public async Task<HttpResponseData> CreateTenant(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tenants")] HttpRequestData req)
    {
        var payload = await JsonSerializer.DeserializeAsync<TenantEntity>(req.Body, JsonOptions);
        if (payload is null || string.IsNullOrWhiteSpace(payload.TenantName))
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new { message = "TenantName is required." });
        }

        db.TenantTble.Add(payload);
        await db.SaveChangesAsync();
        return await CreateJsonResponse(req, HttpStatusCode.Created, payload);
    }

    [Function("GetOwners")]
    public async Task<HttpResponseData> GetOwners(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "owners")] HttpRequestData req)
    {
        var owners = await db.OwnerTble.AsNoTracking().ToListAsync();
        return await CreateJsonResponse(req, HttpStatusCode.OK, owners);
    }

    [Function("CreateOwner")]
    public async Task<HttpResponseData> CreateOwner(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "owners")] HttpRequestData req)
    {
        var payload = await JsonSerializer.DeserializeAsync<OwnerEntity>(req.Body, JsonOptions);
        if (payload is null || string.IsNullOrWhiteSpace(payload.OwnerName))
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new { message = "OwnerName is required." });
        }

        db.OwnerTble.Add(payload);
        await db.SaveChangesAsync();
        return await CreateJsonResponse(req, HttpStatusCode.Created, payload);
    }

    private static async Task<HttpResponseData> CreateJsonResponse<T>(HttpRequestData req, HttpStatusCode code, T payload)
    {
        var response = req.CreateResponse(code);
        await response.WriteAsJsonAsync(payload, JsonOptions);
        return response;
    }
}
