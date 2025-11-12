namespace AdminDashboardApplication.DTOs.SaleItems.Interfaces;

public interface ISaleItemService
{
    Task<SaleItemDto> AddSaleItemAsync(CreateSaleItemDto dto);
    Task<IEnumerable<SaleItemDto>> GetSaleItemsBySaleIdAsync(int saleId);
}