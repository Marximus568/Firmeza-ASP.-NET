using AdminDashboard.Application.DTOs.ExcelImporter;

namespace AdminDashboard.Application.Interfaces.ExcelImporter;

public interface IExcelImporter
{
    Task<ImportResult> ImportClientsAsync(Stream fileStream);
    Task<ImportResult> ImportProductsAsync(Stream fileStream);
    Task<ImportResult> ImportSalesAsync(Stream fileStream);
    Task<ImportResult> ImportSaleItemsAsync(Stream fileStream);
    Task<ImportResult> ImportMixedDataAsync(Stream fileStream);
}