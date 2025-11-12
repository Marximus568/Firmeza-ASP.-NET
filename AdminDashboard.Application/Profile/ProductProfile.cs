using AdminDashboard.Application.Product;
using AdminDashboard.Domain.Entities;

namespace AdminDashboardApplication.Profile;

public class ProductProfile : AutoMapper.Profile
{
    public ProductProfile()
    {
        CreateMap<Products, ProductDto>().ReverseMap();
    }
}