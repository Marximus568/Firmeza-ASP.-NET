using System.Linq;
using System.Collections.Generic;
using AdminDashboard.Domain.Entities;

// NOTE: we'll refer to Contracts types with full qualification to avoid alias/namespace ambiguity

namespace AdminDashboardApplication.DTOs.Users.Mappers
{
    /// <summary>
    /// Maps between Domain.Entities.Users (aliased DomainUser) and Contracts DTOs.
    /// </summary>
    public static class UserMapper
    {
        public static Clients ToEntity(AdminDashboard.Contracts.Users.CreateUserDto dto)
        {
            if (dto == null) return null!;

            return new Clients
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

        public static AdminDashboard.Contracts.Users.UserDto ToDto(Clients client)
        {
            if (client == null) return null!;

            var age = client.DateOfBirth == default ? 0 : (int)((System.DateTime.Today - client.DateOfBirth).TotalDays / 365.25);

            return new AdminDashboard.Contracts.Users.UserDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                FullName = string.Concat(client.FirstName, " ", client.LastName).Trim(),
                Email = client.Email,
                DateOfBirth = client.DateOfBirth,
                Age = age,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                Role = client.Role,
                TotalSales = client.Sales?.Count ?? 0
            };
        }

        public static IEnumerable<AdminDashboard.Contracts.Users.UserDto> ToDtoList(IEnumerable<Clients> users)
            => users?.Select(ToDto).ToList() ?? new List<AdminDashboard.Contracts.Users.UserDto>();

        public static void UpdateEntity(AdminDashboard.Contracts.Users.UpdateUserDto dto, Clients entity)
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
