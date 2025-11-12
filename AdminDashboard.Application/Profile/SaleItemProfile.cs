using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.SaleItems;

namespace AdminDashboardApplication.Profile;

public class SaleItemProfile : AutoMapper.Profile
{
    public SaleItemProfile()
    {
        // Mapping between SaleItems entity and DTOs
        CreateMap<SaleItems, CreateSaleItemDto>().ReverseMap();
        CreateMap<SaleItems, SaleItemDto>().ReverseMap();
    }
}