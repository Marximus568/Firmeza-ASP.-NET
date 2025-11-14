using AdminDashboardApplication.DTOs.Users;

namespace AdminDashboardApplication.Auth.Mappers
{
    public static class UserMapper
    {
        // Map domain user to contract DTO
        public static UserDto ToDto(AdminDashboard.Domain.Entities.Clients clients)
        {
            if (clients == null) return null!;

            // Correct DateOnly age calculation
            int age = 0;
            if (clients.DateOfBirth != default)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                age = today.Year - clients.DateOfBirth.Year;

                // If birthday hasn't happened yet this year → subtract 1
                if (clients.DateOfBirth > today.AddYears(-age))
                    age--;
            }

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
}