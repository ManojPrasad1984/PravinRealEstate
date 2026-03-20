using Microsoft.AspNetCore.Mvc;
using RealEstateManagement.Data;
using RealEstateManagement.Services;

namespace RealEstateManagement.Controllers
{
    public class AgentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AgentService _agentService;

        public AgentController(ApplicationDbContext context, AgentService agentService)
        {
            _context = context;
            _agentService = agentService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Agent model)
        {
            if (ModelState.IsValid)
            {
                model.AgentCode = _agentService.GenerateAgentCode();
                model.CreatedAt = DateTime.Now;

                _context.Agents.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public string GenerateAgentCode()
        {
            var lastAgent = _context.Agents
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastAgent != null && !string.IsNullOrEmpty(lastAgent.AgentCode))
            {
                var numberPart = lastAgent.AgentCode.Replace("AGT", "");
                int.TryParse(numberPart, out nextNumber);
                nextNumber++;
            }

            return "AGT" + nextNumber.ToString("D3");
        }
    }
}
