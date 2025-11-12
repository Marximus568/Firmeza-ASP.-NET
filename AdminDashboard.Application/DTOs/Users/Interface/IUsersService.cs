namespace AdminDashboardApplication.DTOs.Users.Interface
{
    public interface IUsersService
    {
        Task<UserDto> CreateAsync(CreateUserDto createUserDto);
        Task<UserDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> UpdateAsync(UpdateUserDto updateUserDto);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<UserDto> Users, int TotalCount)> SearchAsync(UserFilterDto filter);
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
        Task<IEnumerable<UserDto>> GetByRoleAsync(string role);
        Task<int> GetTotalCountAsync();
    }
}

