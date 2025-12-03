using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using AdminDashboard.Application.DTOs.SalePDF.Reports;

namespace AdminDashboard.Infrastructure.SalePDF.Services;

public class SalesReportDocument : IDocument
{
    private readonly List<SaleReportDto> _sales;

    public SalesReportDocument(List<SaleReportDto> sales)
    {
        _sales = sales;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "Reporte de Ventas",
        Author = "Firmeza Admin Dashboard"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.Size(PageSizes.A4);

            // Header
            page.Header().Column(col =>
            {
                col.Item().Text("REPORTE DE VENTAS")
                    .FontSize(24)
                    .Bold()
                    .FontColor(Colors.Orange.Darken2);
                
                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Medium);
                
                col.Item().PaddingVertical(5).LineHorizontal(2).LineColor(Colors.Orange.Darken2);
            });

            // Content
            page.Content().PaddingVertical(10).Column(col =>
            {
                col.Spacing(10);

                // Summary
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text($"Total de Ventas: {_sales.Count}")
                            .FontSize(12)
                            .Bold();
                        
                        column.Item().Text($"Ingresos Totales: {_sales.Sum(s => s.Total):C}")
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.Green.Darken2);
                        
                        column.Item().Text($"Ventas Pagadas: {_sales.Count(s => s.IsPaid)}")
                            .FontSize(10);
                    });
                });

                col.Item().PaddingVertical(5);

                // Sales table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(35);   // ID
                        cols.RelativeColumn(1);     // Factura
                        cols.RelativeColumn(1);     // Fecha
                        cols.RelativeColumn(2);     // Cliente
                        cols.ConstantColumn(65);    // Total
                        cols.ConstantColumn(55);    // Método
                        cols.ConstantColumn(50);    // Estado
                    });

                    // Table header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Orange.Darken2).Padding(5)
                            .Text("ID").FontColor(Colors.White).Bold().FontSize(9);
                        
                        header.Cell().Background(Colors.Orange.Darken2).Padding(5)
                            .Text("Factura").FontColor(Colors.White).Bold().FontSize(9);
                        
                        header.Cell().Background(Colors.Orange.Darken2).Padding(5)
                            .Text("Fecha").FontColor(Colors.White).Bold().FontSize(9);
                        
                        header.Cell().Background(Colors.Orange.Darken2).Padding(5)
                            .Text("Cliente").FontColor(Colors.White).Bold().FontSize(9);
                        
                        header.Cell().Background(Colors.Orange.Darken2).Padding(5)
                            .Text("Total").FontColor(Colors.White).Bold().FontSize(9);
                        
                        header.Cell().Background(Colors.Orange.Darken2).Padding(5)
                            .Text("Método").FontColor(Colors.White).Bold().FontSize(9);
                        
                        header.Cell().Background(Colors.Orange.Darken2).Padding(5)
                            .Text("Estado").FontColor(Colors.White).Bold().FontSize(9);
                    });

                    // Table rows
                    foreach (var sale in _sales)
                    {
                        var isEven = _sales.IndexOf(sale) % 2 == 0;
                        var bgColor = isEven ? Colors.Grey.Lighten3 : Colors.White;

                        table.Cell().Background(bgColor).Padding(5)
                            .Text(sale.Id.ToString()).FontSize(8);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(sale.InvoiceNumber).FontSize(8);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(sale.SaleDate.ToString("dd/MM/yy")).FontSize(8);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(sale.ClientName).FontSize(8);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(sale.Total.ToString("C")).FontSize(8);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(sale.PaymentMethod).FontSize(8);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(sale.IsPaid ? "Pagado" : "Pendiente")
                            .FontSize(8)
                            .FontColor(sale.IsPaid ? Colors.Green.Darken1 : Colors.Red.Darken1);
                    }
                });
            });

            // Footer
            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("Página ").FontSize(9);
                text.CurrentPageNumber().FontSize(9);
                text.Span(" de ").FontSize(9);
                text.TotalPages().FontSize(9);
            });
        });
    }
}
