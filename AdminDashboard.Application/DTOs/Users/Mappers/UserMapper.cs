using DomainUser = AdminDashboard.Domain.Entities.Users;
using System.Linq;
using System.Collections.Generic;

// NOTE: we'll refer to Contracts types with full qualification to avoid alias/namespace ambiguity

namespace AdminDashboardApplication.DTOs.Users.Mappers
{
    /// <summary>
    /// Maps between Domain.Entities.Users (aliased DomainUser) and Contracts DTOs.
    /// </summary>
    public static class UserMapper
    {
        public static DomainUser ToEntity(AdminDashboard.Contracts.Users.CreateUserDto dto)
        {
            if (dto == null) return null!;

            return new DomainUser
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

        public static AdminDashboard.Contracts.Users.UserDto ToDto(DomainUser user)
        {
            if (user == null) return null!;

            var age = user.DateOfBirth == default ? 0 : (int)((System.DateTime.Today - user.DateOfBirth).TotalDays / 365.25);

            return new AdminDashboard.Contracts.Users.UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = string.Concat(user.FirstName, " ", user.LastName).Trim(),
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Age = age,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = user.Role,
                TotalSales = user.Sales?.Count ?? 0
            };
        }

        public static IEnumerable<AdminDashboard.Contracts.Users.UserDto> ToDtoList(IEnumerable<DomainUser> users)
            => users?.Select(ToDto).ToList() ?? new List<AdminDashboard.Contracts.Users.UserDto>();

        public static void UpdateEntity(AdminDashboard.Contracts.Users.UpdateUserDto dto, DomainUser entity)
        {
            if (dto == null || entity == null) return;

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            entity.DateOfBirth = dto.DateOfBirth;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.Address = dto.Address;
            entity.Role = dto.Role;
        }
    }
}
