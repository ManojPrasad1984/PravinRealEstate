using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using RealEstateManagement.Data;
using RealEstateManagement.Models;
using RealEstateManagement.Services;
using System.Security.Cryptography;
using System.Text;

public class LuckyDrawController : Controller
{
    private readonly ApplicationDbContext _context;

    private string key = "rzp_test_SNeJ15GTPw2Cvx";
    private string secret = "PMfccL3KwBs6pOTnQuE48HKm";

    public LuckyDrawController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Apply()
    {
        return View();
    }

    // Create Razorpay Order
    [HttpPost]
    public IActionResult CreateOrder()
    {
        RazorpayClient client = new RazorpayClient(key, secret);

        Dictionary<string, object> options = new Dictionary<string, object>();

        options.Add("amount", 100); // ₹1 test payment
        options.Add("currency", "INR");
        options.Add("receipt", "LuckyDraw_" + DateTime.Now.Ticks);

        Order order = client.Order.Create(options);

        return Json(new
        {
            orderId = order["id"].ToString()
        });
    }

    // Verify Payment
    [HttpPost]
    public IActionResult VerifyPayment([FromBody] PaymentRequest request)
    {
        var entry = request.Entry;

        entry.PaymentId = request.razorpay_payment_id;
        entry.RazorpayOrderId = request.razorpay_order_id;
        entry.PaymentStatus = true;
        entry.EntryDate = DateTime.Now;
        entry.EntryAmount = 1100;

        entry.CardNumber = GenerateUniqueCardNumber();

        _context.LuckyDrawEntries.Add(entry);
        _context.SaveChanges();

        return Json(new
        {
            success = true,
            card = entry.CardNumber
        });
    }

    // Generate Razorpay Signature
    private string GenerateSignature(string payload)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
        {
            byte[] hash = hmac.ComputeHash(payloadBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    // Generate Card Number
    private string GenerateUniqueCardNumber()
    {
        var last = _context.LuckyDrawEntries.OrderByDescending(x => x.Id).FirstOrDefault();

        int next = last == null ? 1 : last.Id + 1;

        return $"WL-2026-{next:D5}";
    }

    // Generate Receipt
    public IActionResult Receipt(string card)
    {
        var entry = _context.LuckyDrawEntries
            .FirstOrDefault(x => x.CardNumber == card);

        if (entry == null)
            return NotFound();

        return View(entry);
    }
    public IActionResult DownloadReceipt(string card)
    {
        var entry = _context.LuckyDrawEntries
            .AsEnumerable()
            .FirstOrDefault(x => x.CardNumber == card);

        if (entry == null)
            return NotFound();

        //var pdf = LuckyDrawReceiptService.Generate(entry);
        var pdf = PremiumReceiptService.Generate(entry);

        return File(pdf, "application/pdf",
            $"LuckyDraw_{entry.CardNumber}.pdf");
    }
}