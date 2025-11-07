namespace AdminDashboardApplication.DTOs.Users.Interface
{
    public interface IUsersService
    {
        Task<AdminDashboard.Contracts.Users.UserDto> CreateAsync(AdminDashboard.Contracts.Users.CreateUserDto createUserDto);
        Task<AdminDashboard.Contracts.Users.UserDto?> GetByIdAsync(int id);
        Task<IEnumerable<AdminDashboard.Contracts.Users.UserDto>> GetAllAsync();
        Task<AdminDashboard.Contracts.Users.UserDto?> UpdateAsync(AdminDashboard.Contracts.Users.UpdateUserDto updateUserDto);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<AdminDashboard.Contracts.Users.UserDto> Users, int TotalCount)> SearchAsync(AdminDashboard.Contracts.Users.UserFilterDto filter);
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
        Task<IEnumerable<AdminDashboard.Contracts.Users.UserDto>> GetByRoleAsync(string role);
        Task<int> GetTotalCountAsync();
    }
}

