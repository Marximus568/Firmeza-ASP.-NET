using AdminDashboard.Contracts.Users;

namespace AdminDashboardApplication.Auth.Mappers;

public static class UserMapper
{
    // Map domain user to contract DTO
    public static UserDto ToDto(AdminDashboard.Domain.Entities.Clients clients)
    {
        if (clients == null) return null!;

        var age = (int)((DateTime.Today - clients.DateOfBirth).TotalDays / 365.25);

        return new UserDto
        {
            Id = clients.Id,
            FirstName = clients.FirstName,
            LastName = clients.LastName,
            FullName = $"{clients.FirstName} {clients.LastName}".Trim(),
            Email = clients.Email,
            DateOfBirth = clients.DateOfBirth,
            Age = age,
            PhoneNumber = clients.PhoneNumber,
            Address = clients.Address,
            Role = clients.Role,
            TotalSales = clients.Sales?.Count ?? 0
        };
    }
}