using AdminDashboard.Domain.Entities;

namespace AdminDashboardApplication.Profile;

public class SaleProfile : AutoMapper.Profile
{
    public SaleProfile()
    {
        CreateMap<Sales, SaleDto>().ReverseMap();
    }
}