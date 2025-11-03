using AdminDashboard.Application.Users.DTOs;

namespace AdminDashboard.Application.DTOs.User.Interfaces
{
    /// <summary>
    /// Service interface for managing user operations
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="createUserDto">The user data to create</param>
        /// <returns>The created user DTO</returns>
        Task<UserDto> CreateAsync(CreateUserDto createUserDto);

        /// <summary>
        /// Gets a user by their identifier
        /// </summary>
        /// <param name="id">The user identifier</param>
        /// <returns>The user DTO if found, null otherwise</returns>
        Task<UserDto?> GetByIdAsync(int id);

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>A collection of all user DTOs</returns>
        Task<IEnumerable<UserDto>> GetAllAsync();

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="updateUserDto">The updated user data</param>
        /// <returns>The updated user DTO if successful, null otherwise</returns>
        Task<UserDto?> UpdateAsync(UpdateUserDto updateUserDto);

        /// <summary>
        /// Deletes a user by their identifier
        /// </summary>
        /// <param name="id">The user identifier</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Searches and filters users based on provided criteria
        /// </summary>
        /// <param name="filter">The filter criteria</param>
        /// <returns>A paginated collection of user DTOs matching the criteria</returns>
        Task<(IEnumerable<UserDto> Users, int TotalCount)> SearchAsync(UserFilterDto filter);

        /// <summary>
        /// Checks if an email is already in use by another user
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <param name="excludeUserId">Optional user ID to exclude from the check (for updates)</param>
        /// <returns>True if email exists, false otherwise</returns>
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);

        /// <summary>
        /// Gets users by role
        /// </summary>
        /// <param name="role">The role to filter by</param>
        /// <returns>A collection of user DTOs with the specified role</returns>
        Task<IEnumerable<UserDto>> GetByRoleAsync(string role);

        /// <summary>
        /// Gets the total count of users
        /// </summary>
        /// <returns>The total number of users</returns>
        Task<int> GetTotalCountAsync();
    }
}