using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Identity.Entities;

namespace AdminDashboard.Application.Mappers
{
    public static class UserMapper
    {
        // Convert from Identity object to Domain entity
        public static Users ToDomain(ApplicationUserIdentity identityUser)
        {
            if (identityUser == null) return null!;

            return new Users
            {
                // Map Identity Id (string) to Domain Id (int) if needed, or leave for DB auto-gen
                Id = int.TryParse(identityUser.Id, out var parsedId) ? parsedId : 0,
                FirstName = identityUser.UserName ?? string.Empty,
                Email = identityUser.Email ?? string.Empty,
                role = "Client", // Default role, adjust if you have Identity roles
                Phone = string.Empty, // You can map if you add PhoneNumber to domain
                Address = string.Empty // You can map if you add Address to Identity
            };
        }

        // Convert from Domain entity to Identity object
        public static ApplicationUserIdentity ToIdentity(Users users)
        {
            if (users == null) return null!;

            return new ApplicationUserIdentity
            {
                // Keep Id null so Identity can generate or assign it
                UserName = users.FirstName,
                Email = users.Email,
                FirstName = users.FirstName, // Adjust if you split names
                LastName = string.Empty,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                PhoneNumber = users.Phone,
            };
        }
    }
}