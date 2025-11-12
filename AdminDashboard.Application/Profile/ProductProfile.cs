using AdminDashboard.Application.Product;
using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.Products;

namespace AdminDashboardApplication.Profile;

public class ProductProfile : AutoMapper.Profile
{
    public ProductProfile()
    {
        // Mapping between Products entity and DTOs
        CreateMap<Products, CreateProductDto>().ReverseMap();
        CreateMap<Products, UpdateProductDto>().ReverseMap();
        CreateMap<Products, ProductDto>().ReverseMap();
        CreateMap<Products, ProductFilterDto>().ReverseMap();
    }
}