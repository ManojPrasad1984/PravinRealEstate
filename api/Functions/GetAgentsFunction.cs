using System.Net;
using System.Text.Json;
using LuckyDraw.Api.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace LuckyDraw.Api.Functions;

public class GetAgentsFunction
{
    private readonly AppDbContext _dbContext;

    public GetAgentsFunction(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Function("GetAgents")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "agents")] HttpRequestData req)
    {
        var agents = await _dbContext.Agents
            .Where(a => a.IsActive)
            .OrderBy(a => a.AgentName)
            .Select(a => new
            {
                a.Id,
                a.AgentCode,
                a.AgentName,
                DisplayName = $"{a.AgentName} ({a.AgentCode})"
            })
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync(JsonSerializer.Serialize(agents));
        response.Headers.Add("Content-Type", "application/json");
        return response;
    }
}
