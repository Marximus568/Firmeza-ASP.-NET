using AdminDashboard.Application.DTOs.Auth;
using AdminDashboard.Application.Interfaces;
using AdminDashboard.Domain.Entities;

namespace AdminDashboard.Application.UseCases.Auth;

/// <summary>
/// Use case for registering new users.
/// This keeps domain pure and delegates persistence to Infrastructure.
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
    /// Executes the registration process.
    /// </summary>
    public async Task<AuthResultDto> ExecuteAsync(
        RegisterDto registerDto,
        CancellationToken cancellationToken = default)
    {
        // 1️⃣ Check if user already exists
        if (await _authService.UserExistsAsync(registerDto.Email, cancellationToken))
        {
            return AuthResultDto.Failure("A user with this email already exists");
        }

        // 2️⃣ Set default role if not provided
        var role = string.IsNullOrWhiteSpace(registerDto.Role) ? "Client" : registerDto.Role;

        // 3️⃣ Create domain user entity with minimum required info
        var user = new Users
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Role = role
        };

        // 4️⃣ Delegate actual registration to IAuthService
        var result = await _authService.RegisterAsync(user, registerDto.Password, cancellationToken);

        if (!result.Succeeded)
            return result;

        // 5️⃣ Assign role using IRoleService (if needed)
        if (!string.IsNullOrEmpty(result.UserId))
        {
            await _roleService.AssignRoleAsync(result.UserId, role, cancellationToken);
        }

        return result;
    }
}
