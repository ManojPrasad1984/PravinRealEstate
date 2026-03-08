using BarcodeStandard;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using RealEstateManagement.Models;
using SkiaSharp;
using System.Drawing;
using System.IO;

namespace RealEstateManagement.Services
{
    public class PremiumReceiptService
    {
        public static byte[] Generate(LuckyDrawEntry entry)
        {
            var receiptId = $"WRR-{DateTime.Now:yyyyMMdd}-{entry.Id}";

            var qrBytes = GenerateQr($"https://wealthline.com/verify/{entry.CardNumber}");
            var barcodeBytes = GenerateBarcode(entry.CardNumber);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);

                    page.Content().Column(col =>
                    {
                        col.Spacing(15);

                        //--------------------------------
                        // HEADER
                        //--------------------------------

                        col.Item().Background("#0D47A1").Padding(15).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("WEALTHLINE ROYAL RESIDENCES")
                                    .FontColor(Colors.White)
                                    .FontSize(26)
                                    .Bold();

                                c.Item().Text("Grand Lucky Draw Opportunity")
                                    .FontColor(Colors.White)
                                    .FontSize(14);
                            });

                            row.ConstantItem(120)
                                .Background("#FFC107")
                                .Padding(10)
                                .AlignCenter()
                                .AlignMiddle()
                                .Text($"CARD\n{entry.CardNumber}")
                                .FontSize(14)
                                .Bold();
                        });

                        //--------------------------------
                        // RECEIPT TITLE
                        //--------------------------------

                        col.Item().AlignCenter()
                            .Text("Lucky Draw Registration Receipt")
                            .FontSize(20)
                            .Bold();

                        col.Item().LineHorizontal(1);

                        //--------------------------------
                        // DETAILS TABLE
                        //--------------------------------

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(200);
                                c.RelativeColumn();
                            });

                            AddRow(table, "Receipt ID", receiptId);
                            AddRow(table, "Participant Name", entry.FullName);
                            AddRow(table, "Mobile", entry.MobileNumber);
                            AddRow(table, "Address", entry.Address);
                            AddRow(table, "Aadhaar", entry.AadhaarNumber);
                            AddRow(table, "Selected Prize", entry.PrizeChoice);
                            AddRow(table, "Payment ID", entry.PaymentId);
                            AddRow(table, "Payment Date",
                                entry.EntryDate?.ToString("dd MMM yyyy hh:mm tt"));
                        });

                        //--------------------------------
                        // AMOUNT BOX
                        //--------------------------------

                        col.Item().AlignCenter()
                            .Background("#F57C00")
                            .Padding(12)
                            .Text("Registration Amount: ₹1100")
                            .FontSize(22)
                            .Bold()
                            .FontColor(Colors.White);

                        //--------------------------------
                        // QR + BARCODE
                        //--------------------------------

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Verification QR Code")
                                    .Bold();

                                // Fixed: call Width on the container (IContainer) before Image(...)
                                c.Item().Width(120).Image(qrBytes);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Card Barcode")
                                    .Bold();

                                // Fixed: call Width on the container (IContainer) before Image(...)
                                c.Item().Width(200).Image(barcodeBytes);
                            });
                        });

                        //--------------------------------
                        // TERMS
                        //--------------------------------

                        col.Item().Column(c =>
                        {
                            c.Item().Text("Terms & Conditions")
                                .Bold()
                                .FontSize(14);

                            c.Item().Text("• Each participant is eligible for only one prize.");
                            c.Item().Text("• Prize cannot be exchanged for cash.");
                            c.Item().Text("• Winners will be selected through computerized lucky draw.");
                            c.Item().Text("• Organizer decision will be final.");
                        });

                        //--------------------------------
                        // SIGNATURE
                        //--------------------------------

                        col.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeItem();

                            row.ConstantItem(200).Column(c =>
                            {
                                c.Item().LineHorizontal(1);
                                c.Item().AlignCenter()
                                    .Text("Authorized Signature")
                                    .FontSize(10);
                            });
                        });

                        //--------------------------------
                        // FOOTER
                        //--------------------------------

                        col.Item().PaddingTop(20).AlignCenter()
                            .Text("Thank you for participating in Wealthline Royal Residences")
                            .FontSize(10);
                    });
                });
            });

            return document.GeneratePdf();
        }

        static void AddRow(TableDescriptor table, string label, string value)
        {
            table.Cell().BorderBottom(1).Padding(5).Text(label).Bold();
            table.Cell().BorderBottom(1).Padding(5).Text(value ?? "-");
        }

        static byte[] GenerateQr(string text)
        {
            using var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var qr = new PngByteQRCode(data);
            return qr.GetGraphic(20);
        }

        static byte[] GenerateBarcode(string text)
        {
            var barcode = new BarcodeStandard.Barcode();
            var skImage = barcode.Encode(BarcodeStandard.Type.Code128, text, SKColors.Black, SKColors.White, 300, 80);

            using var data = skImage.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
    }
}