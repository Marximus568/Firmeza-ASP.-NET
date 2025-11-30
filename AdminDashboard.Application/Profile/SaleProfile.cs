using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.SaleDate;
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

        // Map from SaleReceiptDto to Sales entity (for frontend checkout)
        CreateMap<SaleReceiptDto, Sales>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Auto-generated
            .ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.ClientId, opt => opt.Ignore()) // Not provided in receipt
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
            .ForMember(dest => dest.TaxRate, opt => opt.MapFrom(src => 0.16m)) // Fixed 16% IVA
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => "Online")) // Default
            .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => true)) // Assume paid
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => $"Customer: {src.CustomerName}, Email: {src.CustomerEmail}"))
            .ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore()) // Auto-generated
            .ForMember(dest => dest.Items, opt => opt.Ignore()) // Handled separately if needed
            .ForMember(dest => dest.Clients, opt => opt.Ignore())
            .ForMember(dest => dest.ReceiptPath, opt => opt.Ignore());
    }
}