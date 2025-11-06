using AdminDashboard.Contracts.Users;
using AdminDashboard.Domain.Entities;


namespace AdminDashboardApplication.DTOs.Auth.Mappers;

public static class UserMapper
{
    // Map domain user to contract DTO
    public static UserDto ToDto(AdminDashboard.Domain.Entities.Users users)
    {
        if (users == null) return null!;

        var age = (int)((DateTime.Today - users.DateOfBirth).TotalDays / 365.25);

        return new UserDto
        {
            Id = users.Id,
            FirstName = users.FirstName,
            LastName = users.LastName,
            FullName = $"{users.FirstName} {users.LastName}".Trim(),
            Email = users.Email,
            DateOfBirth = users.DateOfBirth,
            Age = age,
            PhoneNumber = users.PhoneNumber,
            Address = users.Address,
            Role = users.Role,
            TotalSales = users.Sales?.Count ?? 0
        };
    }
}