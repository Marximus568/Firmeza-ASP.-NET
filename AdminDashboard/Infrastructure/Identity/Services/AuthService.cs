using AdminDashboard.Application.DTOs.Auth;
using AdminDashboard.Application.Interfaces;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace AdminDashboard.Infrastructure.Identity.Services;

/// <summary>
/// Implementation of IAuthService using ASP.NET Core Identity.
/// This adapter connects the Application layer to the Identity framework.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationClientIdentity> _userManager;
    private readonly SignInManager<ApplicationClientIdentity> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        UserManager<ApplicationClientIdentity> userManager,
        SignInManager<ApplicationClientIdentity> signInManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Registers a new client in the system.
    /// </summary>
    public async Task<AuthResultDto> RegisterAsync(
        Clients clients,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationClientIdentity
        {
            UserName = clients.Email,
            Email = clients.Email,
            FirstName = clients.Name,  // Assuming Name maps to FirstName
            LastName = string.Empty,  // Adjust if your domain includes LastName
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return AuthResultDto.Failure(result.Errors.Select(e => e.Description));
        }

        return AuthResultDto.Success(user.Id, user.Email!, Array.Empty<string>());
    }

    /// <summary>
    /// Checks whether a user with the given email already exists.
    /// </summary>
    public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    /// <summary>
    /// Authenticates a user using email and password.
    /// </summary>
    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !user.IsActive)
            return AuthResultDto.Failure("Invalid email or password");

        var result = await _signInManager.PasswordSignInAsync(
            user,
            loginDto.Password,
            loginDto.RememberMe,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
            return AuthResultDto.Failure("Invalid email or password");

        var roles = await _userManager.GetRolesAsync(user);

        return AuthResultDto.Success(user.Id, user.Email!, roles);
    }

    /// <summary>
    /// Logs out the currently authenticated user.
    /// </summary>
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
    }
}
