using RealEstateManagement.Data;

namespace RealEstateManagement.Services
{
    public class AgentService
    {
        private readonly ApplicationDbContext _context;

        public AgentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GenerateAgentCode()
        {
            int count = _context.Agents.Count() + 1;
            return "AGT" + count.ToString("D3"); // AGT001
        }
    }
}
