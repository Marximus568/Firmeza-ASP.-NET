using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.Users;

namespace AdminDashboardApplication.Profile;

public class UserProfile : AutoMapper.Profile
{
    public UserProfile()
    {
        CreateMap<Clients, UserDto>().ReverseMap();
    }
}