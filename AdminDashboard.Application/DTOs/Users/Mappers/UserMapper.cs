using System.Linq;
using System.Collections.Generic;
using AdminDashboard.Domain.Entities;

namespace AdminDashboardApplication.DTOs.Users.Mappers
{
    public static class UserMapper
    {
        // ------------------ CreateUserDto → Clients ------------------

        public static Clients ToEntity(CreateUserDto dto)
        {
            if (dto == null) return null!;

            return new Clients
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,

                // dto.DateOfBirth YA ES DateOnly → no convertir
                DateOfBirth = dto.DateOfBirth,

                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Role = dto.Role ?? "Client"
            };
        }

        // ------------------ Clients → UserDto ------------------

        public static UserDto ToDto(Clients client)
        {
            if (client == null) return null!;

            int age = 0;

            if (client.DateOfBirth != default)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                age = today.Year - client.DateOfBirth.Year;

                if (client.DateOfBirth > today.AddYears(-age))
                    age--;
            }

            return new UserDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                FullName = $"{client.FirstName} {client.LastName}".Trim(),
                Email = client.Email,
                DateOfBirth = client.DateOfBirth,
                Age = age,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                Role = client.Role,
                TotalSales = client.Sales?.Count ?? 0
            };
        }

        // ------------------ List mapper ------------------

        public static IEnumerable<UserDto> ToDtoList(IEnumerable<Clients> users)
            => users?.Select(ToDto).ToList() ?? new List<UserDto>();

        // ------------------ UpdateUserDto → Clients ------------------

        public static void UpdateEntity(UpdateUserDto dto, Clients entity)
        {
            if (dto == null || entity == null) return;

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;

            // dto.DateOfBirth YA ES DateOnly
            entity.DateOfBirth = dto.DateOfBirth;

            entity.PhoneNumber = dto.PhoneNumber;
            entity.Address = dto.Address;
            entity.Role = dto.Role;
        }
    }
}
