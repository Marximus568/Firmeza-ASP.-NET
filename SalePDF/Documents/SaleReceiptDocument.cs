using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SalePDF.DTOs;

namespace SalePDF.Documents;

public class SaleReceiptDocument : IDocument
{
    private readonly SaleReceiptDto _data;

    public SaleReceiptDocument(SaleReceiptDto data)
    {
        _data = data;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("SALES RECEIPT")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().Text("FIRMEZA").FontSize(14).SemiBold();
                column.Item().Text("Sales Management System").FontSize(9).Italic();
            });

            row.RelativeItem().Column(column =>
            {
                column.Item().AlignRight().Text($"Invoice: {_data.InvoiceNumber}").FontSize(12).Bold();
                column.Item().AlignRight().Text($"Sale #{_data.SaleId}").FontSize(10);
                column.Item().AlignRight().Text(_data.Date.ToString("dd/MM/yyyy")).FontSize(10);
                column.Item().AlignRight().Text(_data.Date.ToString("HH:mm:ss")).FontSize(9);
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(15);

            // Client
            column.Item().Element(ComposeClientInfo);

            // Separator line
            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

            // Products
            column.Item().Element(ComposeProductsTable);

            // Totals
            column.Item().Element(ComposeTotals);
        });
    }

    void ComposeClientInfo(IContainer container)
    {
        container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
        {
            column.Item().Text("CUSTOMER DETAILS").FontSize(12).Bold();
            column.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text($"Name: {_data.CustomerName}").FontSize(10);
                row.RelativeItem().Text($"Email: {_data.CustomerEmail}").FontSize(10);
            });
        });
    }

    void ComposeProductsTable(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingBottom(10).Text("PRODUCTS").FontSize(12).Bold();

            column.Item().Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50);  // # 
                    columns.RelativeColumn(4);   // Product
                    columns.ConstantColumn(60);  // Qty
                    columns.ConstantColumn(90);  // Unit Price
                    columns.ConstantColumn(90);  // Subtotal
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("#").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("Product").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("Quantity").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("Unit Price").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("Subtotal").FontColor(Colors.White).Bold();
                });

                // Product rows
                int index = 1;
                foreach (var product in _data.Products)
                {
                    var bgColor = index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                    table.Cell().Background(bgColor).Padding(5).Text(index.ToString());
                    table.Cell().Background(bgColor).Padding(5).Text(product.Name);
                    table.Cell().Background(bgColor).Padding(5).Text(product.Quantity.ToString());
                    table.Cell().Background(bgColor).Padding(5).Text($"${product.UnitPrice:N2}");
                    table.Cell().Background(bgColor).Padding(5).Text($"${product.Subtotal:N2}");

                    index++;
                }
            });
        });
    }

    void ComposeTotals(IContainer container)
    {
        container.AlignRight().Column(column =>
        {
            column.Spacing(5);

            column.Item().Row(row =>
            {
                row.ConstantItem(150).Text("Subtotal:").FontSize(11);
                row.ConstantItem(100).AlignRight().Text($"${_data.Subtotal:N2}").FontSize(11);
            });

            column.Item().Row(row =>
            {
                row.ConstantItem(150).Text($"VAT ({(_data.Iva / _data.Subtotal * 100):N0}%):").FontSize(11);
                row.ConstantItem(100).AlignRight().Text($"${_data.Iva:N2}").FontSize(11);
            });

            column.Item().BorderTop(2).BorderColor(Colors.Blue.Darken2).PaddingTop(5);

            column.Item().Row(row =>
            {
                row.ConstantItem(150).Text("TOTAL:").FontSize(14).Bold();
                row.ConstantItem(100).AlignRight().Text($"${_data.Total:N2}").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Generated on: ").FontSize(8).Italic();
            text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).FontSize(8).Italic().Bold();
        });
    }
}
