using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Identity.Entities;

namespace AdminDashboard.Application.Mappers;

public static class UserMapper
{
    // Convert Identity → Domain
    public static Users ToDomain(ApplicationUserIdentity identityUser)
    {
        if (identityUser == null) return null!;

        return new Users
        {
            Id = int.TryParse(identityUser.Id, out var parsedId) ? parsedId : 0,
            FirstName = identityUser.FirstName,
            Email = identityUser.Email ?? string.Empty,
            Role = identityUser.Role ?? "Client",
            PhoneNumber = identityUser.PhoneNumber ?? string.Empty,
            Address = identityUser.Address ?? string.Empty
        };
    }

    // Convert Domain → Identity
    public static ApplicationUserIdentity ToIdentity(Users users)
    {
        if (users == null) return null!;

        return new ApplicationUserIdentity
        {
            UserName = users.Email,
            Email = users.Email,
            FirstName = users.FirstName,
            LastName = string.Empty,
            Address = users.Address,
            PhoneNumber = users.PhoneNumber,
            Role = users.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}