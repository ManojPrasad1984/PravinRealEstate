using System.Net;
using LuckyDraw.Api.Data;
using LuckyDraw.Api.Helpers;
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
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "agents")] HttpRequestData req)
    {
        if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            return HttpResponseHelper.CreateCorsResponse(req, HttpStatusCode.NoContent);
        }

        try
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

            return await HttpResponseHelper.CreateJsonResponse(req, agents);
        }
        catch (Exception ex)
        {
            return await HttpResponseHelper.CreateJsonResponse(req,
                new { success = false, message = "Failed to load agents.", detail = ex.Message },
                HttpStatusCode.InternalServerError);
        }
    }
}
