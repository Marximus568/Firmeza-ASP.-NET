using AdminDashboard.Application.DTOs.User;
using AdminDashboard.Application.Users.DTOs;
using AdminDashboard.Infrastructure.Identity.Entities;

namespace AdminDashboard.Application.DTOs.Users.Mappers
{
    /// <summary>
    /// Responsible for mapping between Identity, Domain, and DTO objects related to Users.
    /// Keeps the conversion logic centralized and consistent.
    /// </summary>
    public class UserMapper
    {
        // Convert Identity → Domain
        public static Domain.Entities.Users ToDomain(ApplicationUserIdentity identityUser)
        {
            if (identityUser == null) return null!;

            return new Domain.Entities.Users
            {
                Id = int.TryParse(identityUser.Id, out var parsedId) ? parsedId : 0,
                FirstName = identityUser.FirstName,
                LastName = identityUser.LastName ?? string.Empty,
                Email = identityUser.Email ?? string.Empty,
                Role = identityUser.Role ?? "Client",
                PhoneNumber = identityUser.PhoneNumber ?? string.Empty,
                Address = identityUser.Address ?? string.Empty,
                DateOfBirth = identityUser.DateOfBirth
            };
        }

        // Convert Domain → Identity
        public static ApplicationUserIdentity ToIdentity(Domain.Entities.Users user)
        {
            if (user == null) return null!;

            return new ApplicationUserIdentity
            {
                Id = user.Id.ToString(),
                UserName = user.Email,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Convert Domain → DTO
        public static UserDto ToDto(Domain.Entities.Users user)
        {
            if (user == null) return null!;

            var age = (int)((DateTime.Today - user.DateOfBirth).TotalDays / 365.25);

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Age = age,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = user.Role,
                TotalSales = user.Sales?.Count ?? 0
            };
        }

        // Convert list of Domain → DTO
        public static IEnumerable<UserDto> ToDtoList(IEnumerable<Domain.Entities.Users> users)
        {
            return users?.Select(ToDto).ToList() ?? new List<UserDto>();
        }

        // Convert Create DTO → Domain
        public static Domain.Entities.Users ToEntity(CreateUserDto dto)
        {
            return new Domain.Entities.Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Role = dto.Role ?? "Client"
            };
        }

        // Update existing Domain entity from Update DTO
        public static void UpdateEntity(UpdateUserDto dto, Domain.Entities.Users entity)
        {
            // Only overwrite fields that can be updated
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.Address = dto.Address;
            entity.Role = dto.Role;
            entity.DateOfBirth = dto.DateOfBirth;
        }
    }
}
