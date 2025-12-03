using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using AdminDashboard.Application.DTOs.SalePDF.Reports;

namespace AdminDashboard.Infrastructure.SalePDF.Services;

public class ClientsReportDocument : IDocument
{
    private readonly List<ClientReportDto> _clients;

    public ClientsReportDocument(List<ClientReportDto> clients)
    {
        _clients = clients;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "Reporte de Clientes",
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
                col.Item().Text("REPORTE DE CLIENTES")
                    .FontSize(24)
                    .Bold()
                    .FontColor(Colors.Green.Darken2);
                
                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Medium);
                
                col.Item().PaddingVertical(5).LineHorizontal(2).LineColor(Colors.Green.Darken2);
            });

            // Content
            page.Content().PaddingVertical(10).Column(col =>
            {
                col.Spacing(10);

                // Summary
                col.Item().Text($"Total de Clientes: {_clients.Count}")
                    .FontSize(12)
                    .Bold();

                col.Item().PaddingVertical(5);

                // Clients table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(35);   // ID
                        cols.RelativeColumn(2);     // Nombre
                        cols.RelativeColumn(2);     // Email
                        cols.RelativeColumn(1);     // Teléfono
                        cols.ConstantColumn(60);    // Ventas
                    });

                    // Table header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Green.Darken2).Padding(5)
                            .Text("ID").FontColor(Colors.White).Bold().FontSize(10);
                        
                        header.Cell().Background(Colors.Green.Darken2).Padding(5)
                            .Text("Nombre Completo").FontColor(Colors.White).Bold().FontSize(10);
                        
                        header.Cell().Background(Colors.Green.Darken2).Padding(5)
                            .Text("Email").FontColor(Colors.White).Bold().FontSize(10);
                        
                        header.Cell().Background(Colors.Green.Darken2).Padding(5)
                            .Text("Teléfono").FontColor(Colors.White).Bold().FontSize(10);
                        
                        header.Cell().Background(Colors.Green.Darken2).Padding(5)
                            .Text("Ventas").FontColor(Colors.White).Bold().FontSize(10);
                    });

                    // Table rows
                    foreach (var client in _clients)
                    {
                        var isEven = _clients.IndexOf(client) % 2 == 0;
                        var bgColor = isEven ? Colors.Grey.Lighten3 : Colors.White;

                        table.Cell().Background(bgColor).Padding(5)
                            .Text(client.Id.ToString()).FontSize(9);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(client.FullName).FontSize(9);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(client.Email).FontSize(9);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(client.PhoneNumber).FontSize(9);
                        
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(client.TotalSales.ToString()).FontSize(9);
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
