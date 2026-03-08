using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RealEstateManagement.Models;

public class LuckyDrawReceiptService
{
    public static byte[] Generate(LuckyDrawEntry entry)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);

                page.Content().Column(col =>
                {
                    col.Spacing(15);

                    //------------------------------------
                    // HEADER
                    //------------------------------------

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Wealthline Infrastructure")
                                .FontSize(22)
                                .Bold()
                                .FontColor("#1A237E");

                            c.Item().Text("Creating wealth for your future")
                                .FontSize(10);

                            c.Item().Text("NIT Commercial Complex, LGF B-9")
                                .FontSize(10);

                            c.Item().Text("Sitabuldi, Nagpur")
                                .FontSize(10);

                            c.Item().Text("Mob: 7843045164, 8830303318")
                                .FontSize(10);
                        });

                        row.ConstantItem(150)
                            .Border(2)
                            .Padding(10)
                            .AlignCenter()
                            .AlignMiddle()
                            .Text($"Card No.\n{entry.CardNumber}")
                            .FontSize(14)
                            .Bold();
                    });

                    //------------------------------------
                    // MAIN TITLE
                    //------------------------------------

                    col.Item()
                        .AlignCenter()
                        .Background("#D32F2F")
                        .PaddingVertical(8)
                        .Text("परमाला एक नगरी")
                        .FontColor(Colors.White)
                        .FontSize(28)
                        .Bold();

                    col.Item()
                        .AlignCenter()
                        .Background("#FBC02D")
                        .Padding(6)
                        .Text("आकर्षक इनामी ड्रॉ योजना")
                        .FontSize(18)
                        .Bold();

                    //------------------------------------
                    // NAME LINE
                    //------------------------------------

                    col.Item().PaddingTop(10)
                        .Row(row =>
                        {
                            row.ConstantItem(120)
                                .Text("श्री / श्रीमती")
                                .FontSize(14);

                            row.RelativeItem()
                                .BorderBottom(1)
                                .PaddingBottom(3)
                                .Text(entry.FullName)
                                .FontSize(16)
                                .Bold();
                        });

                    //------------------------------------
                    // DRAW DATE
                    //------------------------------------

                    col.Item().PaddingTop(15)
                        .AlignCenter()
                        .Text("26 जनवरी 2026 से 30 अप्रैल 2026 तक")
                        .FontSize(18)
                        .Bold()
                        .FontColor("#1A237E");

                    col.Item()
                        .AlignCenter()
                        .Text("ड्रॉ खुलने की तारीख - रविवार 10 मई 2026")
                        .FontSize(14)
                        .FontColor("#D32F2F");

                    //------------------------------------
                    // PRICE BOX
                    //------------------------------------

                    col.Item()
                        .PaddingTop(20)
                        .AlignCenter()
                        .Background("#FB8C00")
                        .Padding(12)
                        .Text("₹ 1100 /-")
                        .FontSize(26)
                        .Bold()
                        .FontColor(Colors.White);

                    //------------------------------------
                    // FOOTER
                    //------------------------------------

                    col.Item()
                        .PaddingTop(25)
                        .AlignCenter()
                        .Text("Thank you for participating in Wealthline Lucky Draw")
                        .FontSize(11);
                });
            });
        });

        return doc.GeneratePdf();
    }
}