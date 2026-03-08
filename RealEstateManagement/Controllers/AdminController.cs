using Microsoft.AspNetCore.Mvc;
using RealEstateManagement.Data;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult DrawWinners()
    {
        var winners = _context.LuckyDrawEntries
            .Where(x => x.PaymentStatus == true)
            .OrderBy(x => Guid.NewGuid())
            .Take(10)
            .ToList();

        return View(winners);
    }
}