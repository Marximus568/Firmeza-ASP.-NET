using AdminDashboard.Identity.Entities;
using AdminDashboardApplication.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AdminDashboard.Identity.Services;

/// <summary>
/// Handles role assignment and role checking for users.
/// Implements IRoleService contract.
/// </summary>
public class RoleService : IRoleService
{
    private readonly UserManager<ApplicationUserIdentity> _userManager;

    public RoleService(UserManager<ApplicationUserIdentity> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Assigns a role to a specific user.
    /// </summary>
    public async Task AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException($"User with ID '{userId}' not found.");

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(role))
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign role: {errors}");
            }
        }
    }

    /// <summary>
    /// Checks if a user has a specific role.
    /// </summary>
    public async Task<bool> HasRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains(role);
    }
}
