using AdminDashboard.Application.DTOs.Auth;
using AdminDashboard.Contracts.Users;

namespace AdminDashboardApplication.DTOs.Auth.Interfaces;

/// <summary>
/// Defines the contract for authentication operations.
/// Implemented in the Infrastructure layer (e.g., Identity, JWT, etc.)
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new client in the system.
    /// </summary>
    /// <param name="userDto">The user DTO to register.</param>
    /// <param name="password">The plain text password to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An AuthResultDto indicating success or failure.</returns>
    Task<AuthResultDto> RegisterAsync(UserDto userDto, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a user with the given email already exists.
    /// </summary>
    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user using email and password.
    /// </summary>
    Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out the currently authenticated user.
    /// </summary>
    Task LogoutAsync(CancellationToken cancellationToken = default);
}