using AdminDashboard.Domain.Entities;

namespace AdminDashboardApplication.Profile;

public class SaleItemProfile : AutoMapper.Profile
{
    public SaleItemProfile()
    {
        CreateMap<SaleItems, SaleItemDto>().ReverseMap();
    }
}