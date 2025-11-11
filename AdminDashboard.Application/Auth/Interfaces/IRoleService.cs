namespace AdminDashboardApplication.Auth.Interfaces;

/// <summary>
/// Defines operations related to roles and permissions.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Assigns a role to a specific user.
    /// </summary>
    Task AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role.
    /// </summary>
    Task<bool> HasRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
}