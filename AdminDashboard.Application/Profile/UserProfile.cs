using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.Users;

namespace AdminDashboardApplication.Profile;

public class UserProfile : AutoMapper.Profile
{
    public UserProfile()
    {
        // Mapping between Users entity and DTOs
        CreateMap<Clients, CreateUserDto>().ReverseMap();
        CreateMap<Clients, UserFilterDto>().ReverseMap();
        CreateMap<Clients, UserDto>().ReverseMap();
        CreateMap<Clients, UpdateUserDto>().ReverseMap();
    }
}