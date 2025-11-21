using AdminDashboardApplication.DTOs.SaleDate;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Firmeza.WebApi.wwwroot.receipt;

public interface IPdfService
{
    string GenerateReceiptPdf(SaleReceiptDto data);
}

public class PdfService : IPdfService
{
    private readonly IWebHostEnvironment _env;

    public PdfService(IWebHostEnvironment env)
    {
        _env = env;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public string GenerateReceiptPdf(SaleReceiptDto data)
    {
        var receiptsDir = Path.Combine(_env.WebRootPath, "receipts");
        if (!Directory.Exists(receiptsDir))
            Directory.CreateDirectory(receiptsDir);

        var fileName = $"receipt_{data.SaleId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        var filePath = Path.Combine(receiptsDir, fileName);

        var document = new ReceiptDocument(data);
        var pdfBytes = document.GeneratePdf();

        File.WriteAllBytes(filePath, pdfBytes);

        return $"/receipts/{fileName}";
    }
}

public class ReceiptDocument : IDocument
{
    private readonly SaleReceiptDto _data;

    public ReceiptDocument(SaleReceiptDto data)
    {
        _data = data;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "Sales Receipt",
        Author = "Firmeza System"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);

            page.Header().Text("SALES RECEIPT")
                .FontSize(24)
                .Bold()
                .AlignCenter();

            page.Content().Column(col =>
            {
                col.Spacing(10);

                // Customer information
                col.Item().Text($"Customer: {_data.CustomerName}");
                col.Item().Text($"Email: {_data.CustomerEmail}");
                col.Item().Text($"Sale Number: {_data.SaleId}");
                col.Item().Text($"Date: {_data.Date:yyyy-MM-dd HH:mm}");

                col.Item().LineHorizontal(1).LineColor("#E0E0E0");

                // Products table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn();
                        cols.ConstantColumn(40);
                        cols.ConstantColumn(80);
                        cols.ConstantColumn(80);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Product").Bold();
                        header.Cell().Text("Qty").Bold();
                        header.Cell().Text("Unit Price").Bold();
                        header.Cell().Text("Total").Bold();
                    });

                    foreach (var p in _data.Products)
                    {
                        table.Cell().Text(p.Name);
                        table.Cell().Text(p.Qty.ToString());
                        table.Cell().Text(p.UnitPrice.ToString("C"));
                        table.Cell().Text(p.Total.ToString("C"));
                    }
                });

                col.Item().LineHorizontal(1).LineColor("#E0E0E0");

                // Totals section
                col.Item().Text($"Subtotal: {_data.Subtotal:C}");
                col.Item().Text($"IVA: {_data.Iva:C}");
                col.Item().Text($"TOTAL: {_data.Total:C}")
                    .FontSize(16)
                    .Bold();
            });

            page.Footer().AlignCenter().Text(txt =>
            {
                txt.Span("Generated automatically by Firmeza System").FontSize(10);
            });
        });
    }
}
