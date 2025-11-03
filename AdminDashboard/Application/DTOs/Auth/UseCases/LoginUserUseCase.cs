using AdminDashboard.Application.DTOs.Auth;
using AdminDashboard.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace AdminDashboard.Application.UseCases.Auth
{
    /// <summary>
    /// Handles the user login use case.
    /// </summary>
    public class LoginUserUseCase
    {
        private readonly IAuthService _authService;

        public LoginUserUseCase(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Executes the login process for a user.
        /// </summary>
        /// <param name="loginDto">The login data (email and password)</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>AuthResultDto with the login outcome</returns>
        public async Task<AuthResultDto> ExecuteAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            // Optional: validate input
            if (string.IsNullOrWhiteSpace(loginDto.Email))
                return AuthResultDto.Failure("Email is required.");

            if (string.IsNullOrWhiteSpace(loginDto.Password))
                return AuthResultDto.Failure("Password is required.");

            // Delegate authentication to AuthService
            var result = await _authService.LoginAsync(loginDto, cancellationToken);

            return result;
        }
    }
}