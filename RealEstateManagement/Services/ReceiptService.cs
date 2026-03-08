using QuestPDF.Fluent;
using RealEstateManagement.Models;

public class ReceiptService
{

    public static byte[] GenerateReceipt(LuckyDrawEntry entry)
    {

        var pdf = Document.Create(container =>
        {

            container.Page(page =>
            {

                page.Content().Column(col =>
                {

                    col.Item().Text("Wealthline Infrastructure")
                    .FontSize(22).Bold();

                    col.Item().Text("Lucky Draw Scheme Receipt");

                    col.Item().Text($"Name: {entry.FullName}");

                    col.Item().Text($"Mobile: {entry.MobileNumber}");

                    col.Item().Text($"Prize: {entry.PrizeChoice}");

                    col.Item().Text($"Payment ID: {entry.PaymentId}");

                    col.Item().Text($"Amount Paid: ₹1");

                });

            });

        }).GeneratePdf();

        return pdf;

    }

}