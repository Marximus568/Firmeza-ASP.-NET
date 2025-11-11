using System;
using System.Threading;
using System.Threading.Tasks;
using AdminDashboardApplication.Auth.Interfaces;

namespace AdminDashboard.Application.UseCases.Auth
{
    /// <summary>
    /// Use case for assigning a role to a user.
    /// </summary>
    public class AssignRoleUseCase
    {
        private readonly IRoleService _roleService;

        public AssignRoleUseCase(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Executes the role assignment for a specific user.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="role">The role to assign</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        public async Task ExecuteAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID must be provided.", nameof(userId));

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role must be provided.", nameof(role));

            // Delegate the role assignment to the RoleService
            await _roleService.AssignRoleAsync(userId, role, cancellationToken);
        }
    }
}