using AdminDashboard.Application.DTOs.Auth;
using AdminDashboard.Application.Interfaces;
using AdminDashboard.Domain.Entities;

namespace AdminDashboard.Application.UseCases.Auth;

/// <summary>
/// Use case for registering new clients in the system.
/// Handles the business logic for client registration.
/// </summary>
public class RegisterUserUseCase
{
    private readonly IAuthService _authService;
    private readonly IRoleService _roleService;

    public RegisterUserUseCase(IAuthService authService, IRoleService roleService)
    {
        _authService = authService;
        _roleService = roleService;
    }

    /// <summary>
    /// Executes the client registration process.
    /// </summary>
    public async Task<AuthResultDto> ExecuteAsync(
        RegisterDto registerDto,
        CancellationToken cancellationToken = default)
    {
        // 1️⃣ Verify if the client already exists by email
        if (await _authService.UserExistsAsync(registerDto.Email, cancellationToken))
        {
            return AuthResultDto.Failure("A client with this email already exists");
        }

        // 2️⃣ Define the role — default to "Client" if not provided
        var role = string.IsNullOrWhiteSpace(registerDto.Role)
            ? "Client"
            : registerDto.Role;

        // 3️⃣ Create domain client instance
        var client = new Users
        {
            FirstName = $"{registerDto.FirstName} {registerDto.LastName}".Trim(),
            Email = registerDto.Email,
            role = role,
            Address = string.Empty,
            Phone = string.Empty
        };

        // 4️⃣ Delegate registration to AuthService (Identity logic handled internally)
        var result = await _authService.RegisterAsync(client, registerDto.Password, cancellationToken);

        if (!result.Succeeded)
            return result;

        // 5️⃣ Assign role (if AuthService doesn’t handle this internally)
        if (!string.IsNullOrEmpty(result.UserId))
            await _roleService.AssignRoleAsync(result.UserId, role, cancellationToken);

        return result;
    }
}