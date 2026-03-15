using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using RealEstateManagement.Data;
using RealEstateManagement.Models;
using RealEstateManagement.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class LuckyDrawController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    private readonly string _razorKey;
    private readonly string _razorSecret;
    private readonly int _entryAmount;

    public LuckyDrawController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;

        _razorKey = _config["Razorpay:Key"];
        _razorSecret = _config["Razorpay:Secret"];
        _entryAmount = Convert.ToInt32(_config["LuckyDraw:EntryAmount"]);
    }

    public IActionResult Apply()
    {
        ViewBag.EntryAmount = _entryAmount;
        ViewBag.RazorKey = _razorKey;
        return View();
    }

    // Create Razorpay Order
    [HttpPost]
    public IActionResult CreateOrder()
    {
        RazorpayClient client = new RazorpayClient(_razorKey, _razorSecret);

        Dictionary<string, object> options = new Dictionary<string, object>
        {
            { "amount", _entryAmount * 100 },
            { "currency", "INR" },
            { "receipt", "LuckyDraw_" + DateTime.Now.Ticks }
        };

        Order order = client.Order.Create(options);

        return Json(new
        {
            orderId = order["id"].ToString(),
            amount = _entryAmount
        });
    }

    [HttpPost]
    public IActionResult VerifyPayment([FromBody] PaymentRequest request)
    {
        try
        {
            if (request?.Entry == null)
                return Json(new { success = false, message = "Invalid request." });

            var entry = request.Entry;

            // VALIDATIONS
            if (string.IsNullOrWhiteSpace(entry.FullName))
                return Json(new { success = false, message = "Full Name is required." });

            if (!Regex.IsMatch(entry.MobileNumber, "^[6-9][0-9]{9}$"))
                return Json(new { success = false, message = "Invalid mobile number." });

            if (!Regex.IsMatch(entry.AadhaarNumber, "^[0-9]{12}$"))
                return Json(new { success = false, message = "Invalid Aadhaar number." });

            //bool aadhaarExists = _context.LuckyDrawEntries
            //    .Any(x => x.AadhaarNumber == entry.AadhaarNumber);

            //if (aadhaarExists)
            //    return Json(new { success = false, message = "Aadhaar already registered." });

            bool paymentExists = _context.LuckyDrawEntries
                .Any(x => x.PaymentId == request.razorpay_payment_id);

            if (paymentExists)
                return Json(new { success = false, message = "Payment already processed." });

            // VERIFY SIGNATURE
            string payload = request.razorpay_order_id + "|" + request.razorpay_payment_id;
            string signature = GenerateSignature(payload);

            if (signature != request.razorpay_signature)
                return Json(new { success = false, message = "Payment verification failed." });

            entry.PaymentId = request.razorpay_payment_id;
            entry.RazorpayOrderId = request.razorpay_order_id;
            entry.PaymentStatus = true;
            entry.EntryDate = DateTime.UtcNow;
            entry.EntryAmount = _entryAmount;
            entry.PrizeChoice = entry.PrizeChoice ?? "No Prize";
            entry.CardNumber = GenerateUniqueCardNumber();

            _context.LuckyDrawEntries.Add(entry);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                card = entry.CardNumber
            });
        }
        catch
        {
            return Json(new
            {
                success = false,
                message = "Unexpected error occurred."
            });
        }
    }

    private string GenerateSignature(string payload)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(_razorSecret);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);

        return BitConverter.ToString(hmac.ComputeHash(payloadBytes))
            .Replace("-", "")
            .ToLower();
    }

    private string GenerateUniqueCardNumber()
    {
        var last = _context.LuckyDrawEntries
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        int next = last == null ? 1 : last.Id + 1;

        return $"WL-{DateTime.Now.Year}-{next:D5}";
    }

    public IActionResult DownloadReceipt(string card)
    {
        var entry = _context.LuckyDrawEntries
            .FirstOrDefault(x => x.CardNumber == card);

        if (entry == null)
            return NotFound();

        var pdf = PremiumReceiptService.Generate(entry);

        return File(pdf, "application/pdf",
            $"LuckyDraw_{entry.CardNumber}.pdf");
    }
}