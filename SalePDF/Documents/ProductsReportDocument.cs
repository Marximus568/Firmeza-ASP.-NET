using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SalePDF.DTOs.Reports;

namespace SalePDF.Services;

public class ProductsReportDocument : IDocument
{
    private readonly List<ProductReportDto> _products;

    public ProductsReportDocument(List<ProductReportDto> products)
    {
        _products = products;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "Reporte de Productos",
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
                col.Item().Text("REPORTE DE PRODUCTOS")
                    .FontSize(24)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);
                
                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Medium);
                
                col.Item().PaddingVertical(5).LineHorizontal(2).LineColor(Colors.Blue.Darken2);
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
                        column.Item().Text($"Total de Productos: {_products.Count}")
                            .FontSize(12)
                            .Bold();
                        
                        column.Item().Text($"Valor Total Inventario: {_products.Sum(p => p.UnitPrice * p.Stock):C}")
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.Green.Darken2);
                    });
                });

                col.Item().PaddingVertical(5);

                // Products table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(40);  // ID
                        cols.RelativeColumn(3);    // Nombre
                        cols.RelativeColumn(2);    // Categoría
                        cols.ConstantColumn(80);   // Precio
                        cols.ConstantColumn(60);   // Stock
                    });

                    // Table header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                            .Text("ID").FontColor(Colors.White).Bold().FontSize(10);
                        
                        header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                            .Text("Nombre").FontColor(Colors.White).Bold().FontSize(10);
                        
                        header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                            .Text("Categoría").FontColor(Colors.White).Bold().FontSize(10);
                        
                        header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                            .Text("Precio Unit.").FontColor(Colors.White).Bold().FontSize(10);
                        
                        header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                            .Text("Stock").FontColor(Colors.White).Bold().FontSize(10);
                    });

                    // Table rows
                    foreach (var product in _products)
                    {
                        var isEven = _products.IndexOf(product) % 2 == 0;
                        var bgColor = isEven ? Colors.Grey.Lighten3 : Colors.White;

                        table.Cell().Background(bgColor).Padding(5)
                            .Text(product.Id.ToString()).FontSize(9);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(product.Name).FontSize(9);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(product.CategoryName).FontSize(9);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(product.UnitPrice.ToString("C")).FontSize(9);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(product.Stock.ToString()).FontSize(9);
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
