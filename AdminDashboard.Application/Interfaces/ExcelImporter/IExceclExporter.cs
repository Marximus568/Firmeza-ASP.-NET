using System.IO;
using System.Threading.Tasks;

namespace AdminDashboard.Application.Interfaces.ExcelImporter
{
    /// <summary>
    /// Defines methods for exporting domain data into Excel files.
    /// </summary>
    public interface IExcelExporter
    {
        Task<byte[]> ExportClientsAsync();
        Task<byte[]> ExportProductsAsync();
        Task<byte[]> ExportSalesAsync();
        Task<byte[]> ExportSaleItemsAsync();
    }
}