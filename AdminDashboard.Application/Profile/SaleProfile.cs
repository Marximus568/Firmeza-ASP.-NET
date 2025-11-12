using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.SaleItems;
using AdminDashboardApplication.DTOs.Sales;

namespace AdminDashboardApplication.Profile;

public class SaleProfile : AutoMapper.Profile
{
    public SaleProfile()
    {
        // Map from entity to DTO and vice versa
        CreateMap<Sales, CreateSaleDto>().ReverseMap();
        CreateMap<Sales, UpdateSaleDto>().ReverseMap();

        // Used when returning detailed sale info
        CreateMap<Sales, SaleResponseDto>().ReverseMap();

        // Used when summarizing sales with related SaleItems
        CreateMap<Sales, SaleItemSummaryDto>().ReverseMap();
    }
}