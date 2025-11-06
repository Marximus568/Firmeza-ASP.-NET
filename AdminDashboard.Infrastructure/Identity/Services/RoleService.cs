using AdminDashboard.Application.Interfaces;

namespace AdminDashboard.Infrastructure.Identity.Services;

[System.Obsolete("RoleService implementation moved to AdminDashboard.Identity.Services.RoleService. Remove this placeholder after migration.")]
public class RoleServicePlaceholder : IRoleService
{
    public Task AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<bool> HasRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}