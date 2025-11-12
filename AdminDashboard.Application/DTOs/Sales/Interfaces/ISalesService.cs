using AdminDashboardApplication.DTOs.Sales;

namespace AdminDashboard.Application.Interfaces;

using AdminDashboard.Application.DTOs.Sales;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISalesService
{
    Task<SaleResponseDto> CreateSaleAsync(CreateSaleDto dto);
    Task<SaleResponseDto?> GetSaleByIdAsync(int id);
    Task<IEnumerable<SaleResponseDto>> GetAllSalesAsync();
    Task<SaleResponseDto> UpdateSaleAsync(UpdateSaleDto dto);
    Task<bool> DeleteSaleAsync(int id);
}