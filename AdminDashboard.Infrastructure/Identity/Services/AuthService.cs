using AdminDashboard.Application.DTOs.Auth;
using AdminDashboard.Application.Interfaces;
using AdminDashboard.Contracts.Users;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Identity.Entities;
using AdminDashboardApplication.DTOs.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AdminDashboard.Infrastructure.Identity.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUserIdentity> _userManager;
    private readonly SignInManager<ApplicationUserIdentity> _signInManager;

    public AuthService(
        UserManager<ApplicationUserIdentity> userManager,
        SignInManager<ApplicationUserIdentity> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AuthResultDto> RegisterAsync(UserDto userDto, string password, CancellationToken cancellationToken = default)
    {
        var users = new Users
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            Role = userDto.Role,
            PhoneNumber = userDto.PhoneNumber,
            Address = userDto.Address
        };

        var identityUser = new ApplicationUserIdentity
        {
            UserName = users.Email,
            Email = users.Email,
            FirstName = users.FirstName,
            LastName = users.LastName,
            PhoneNumber = users.PhoneNumber,
            Address = users.Address,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(identityUser, password);

        if (!result.Succeeded)
            return AuthResultDto.Failure(result.Errors.Select(e => e.Description));

        if (!string.IsNullOrEmpty(users.Role))
            await _userManager.AddToRoleAsync(identityUser, users.Role);

        return AuthResultDto.Success(identityUser.Id, identityUser.Email!, Array.Empty<string>());
    }
    public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

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

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
    }
}
