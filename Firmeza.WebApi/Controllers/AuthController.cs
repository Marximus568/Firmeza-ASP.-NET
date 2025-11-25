using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdminDashboardApplication.Auth.UseCases;
using AdminDashboardApplication.Auth;
using AdminDashboardApplication.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace Firmeza.WebApi.Controllers;

[ApiController]
[Route("v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly LoginUserUseCase _loginUseCase;
    private readonly RegisterUserUseCase _registerUseCase;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        LoginUserUseCase loginUseCase,
        RegisterUserUseCase registerUseCase,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _loginUseCase = loginUseCase;
        _registerUseCase = registerUseCase;
        _configuration = configuration;
        _logger = logger;
    }

    // =========================================================
    // POST: v1/auth/login
    // =========================================================
    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">Login credentials (email and password)</param>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "User login", 
        Description = "Authenticates user credentials and returns a JWT token with user information.")]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _loginUseCase.ExecuteAsync(loginDto);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", loginDto.Email);
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Generate JWT token
        var token = GenerateJwtToken(result);

        _logger.LogInformation("User {Email} logged in successfully", result.Email);

        return Ok(new LoginResponseDto
        {
            Token = token,
            User = new UserInfoDto
            {
                Email = result.Email!,
                UserId = result.UserId!,
                Role = result.Roles.FirstOrDefault() ?? "Client"
            }
        });
    }

    // =========================================================
    // POST: v1/auth/register
    // =========================================================
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Register new user", 
        Description = "Creates a new user account and returns a JWT token.")]
    [ProducesResponseType(typeof(LoginResponseDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _registerUseCase.ExecuteAsync(registerDto);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed registration attempt for email: {Email}. Errors: {Errors}", 
                registerDto.Email, string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        // Generate JWT token for the newly registered user
        var token = GenerateJwtToken(result);

        _logger.LogInformation("New user registered: {Email}", result.Email);

        return CreatedAtAction(nameof(Login), new LoginResponseDto
        {
            Token = token,
            User = new UserInfoDto
            {
                Email = result.Email!,
                UserId = result.UserId!,
                Role = result.Roles.FirstOrDefault() ?? "Client"
            }
        });
    }

    // =========================================================
    // Helper: Generate JWT Token
    // =========================================================
    private string GenerateJwtToken(AuthResultDto authResult)
    {
        var key = _configuration["Jwt:Key"]
                  ?? Environment.GetEnvironmentVariable("Jwt__Key")
                  ?? throw new Exception("JWT Key not found in configuration");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, authResult.Email!),
            new Claim(JwtRegisteredClaimNames.Email, authResult.Email!),
            new Claim(ClaimTypes.NameIdentifier, authResult.UserId!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add role claims
        foreach (var role in authResult.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "Firmeza.WebApi",
            audience: _configuration["Jwt:Audience"] ?? "Firmeza.Users",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// =========================================================
// DTOs for Authentication Responses
// =========================================================

/// <summary>
/// Response DTO for login/register endpoints
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserInfoDto User { get; set; } = new();
}

/// <summary>
/// User information included in auth response
/// </summary>
public class UserInfoDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
