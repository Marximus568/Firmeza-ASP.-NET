using ExcelImporter.Models;

namespace ExcelImporter.Interfaces;

public interface IExcelImporter
{
    Task<ImportResult> ImportClientsAsync(Stream fileStream);
    Task<ImportResult> ImportProductsAsync(Stream fileStream);
    Task<ImportResult> ImportSalesAsync(Stream fileStream);
    Task<ImportResult> ImportSaleItemsAsync(Stream fileStream);
    Task<ImportResult> ImportMixedDataAsync(Stream fileStream);
}