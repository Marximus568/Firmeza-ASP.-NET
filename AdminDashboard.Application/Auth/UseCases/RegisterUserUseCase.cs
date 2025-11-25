using AdminDashboardApplication.Auth.Interfaces;
using AdminDashboardApplication.DTOs.Users;

namespace AdminDashboardApplication.Auth.UseCases;

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
        // 1 Check if user already exists
        if (await _authService.UserExistsAsync(registerDto.Email, cancellationToken))
        {
            return AuthResultDto.Failure("A user with this email already exists");
        }

        // 2 Set default role if not provided
        var role = string.IsNullOrWhiteSpace(registerDto.Role) ? "Client" : registerDto.Role;

        // 3 Create user DTO (now using centralized Contracts DTO)
        var user = new UserDto
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Role = role
        };

        // 4 Delegate actual registration to IAuthService
        var result = await _authService.RegisterAsync(user, registerDto.Password, cancellationToken);

        if (!result.Succeeded)
            return result;

        // 5 Assign role using IRoleService (if needed)
        if (!string.IsNullOrEmpty(result.UserId))
        {
            await _roleService.AssignRoleAsync(result.UserId, role, cancellationToken);
        }

        return result;
    }
}
