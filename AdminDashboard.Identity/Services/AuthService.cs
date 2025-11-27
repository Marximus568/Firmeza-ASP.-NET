using AdminDashboard.Domain.Entities;
using AdminDashboard.Identity.Entities;
using AdminDashboardApplication.Auth;
using AdminDashboardApplication.Auth.Interfaces;
using AdminDashboardApplication.DTOs.Users;
using AdminDashboardApplication.Interfaces;
using AdminDashboardApplication.Interfaces.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AdminDashboard.Identity.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUserIdentity> _userManager;
    private readonly SignInManager<ApplicationUserIdentity> _signInManager;
    private readonly IEmailService _emailService;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<AuthService> _logger;

    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        UserManager<ApplicationUserIdentity> userManager,
        SignInManager<ApplicationUserIdentity> signInManager,
        IEmailService emailService,
        ICustomerRepository customerRepository,
        ILogger<AuthService> logger,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _customerRepository = customerRepository;
        _logger = logger;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResultDto> RegisterAsync(UserDto userDto, string password, CancellationToken cancellationToken = default)
    {
        var users = new Clients
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            Role = userDto.Role,
            PhoneNumber = userDto.PhoneNumber,
            Address = userDto.Address,
            Password = password
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

        // Save client to business database
        try 
        {
            await _customerRepository.AddCustomerAsync(users);
            _logger.LogInformation("Client {Email} saved to business database", users.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save client {Email} to business database", users.Email);
            // We might want to rollback identity creation here, but for now let's log it
            // Ideally this should be in a transaction
        }

        // Send welcome email
        try
        {
            await SendWelcomeEmailAsync(identityUser, users.Role);
            _logger.LogInformation("Welcome email sent successfully to {Email}", identityUser.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}. Registration completed successfully.", identityUser.Email);
            // Don't fail registration if email fails
        }

        var roles = new[] { users.Role ?? "Client" };
        var token = _jwtTokenGenerator.GenerateToken(identityUser, roles);

        return AuthResultDto.Success(identityUser.Id, identityUser.Email!, roles, token);
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
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        return AuthResultDto.Success(user.Id, user.Email!, roles, token);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
    }

    private async Task SendWelcomeEmailAsync(ApplicationUserIdentity user, string role)
    {
        var subject = "Welcome to Firmeza Admin!";
        var body = CreateWelcomeEmailTemplate(user.FirstName, user.LastName, user.Email!, role);
        
        await _emailService.SendEmailAsync(user.Email!, subject, body);
    }

    private string CreateWelcomeEmailTemplate(string firstName, string lastName, string email, string role)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f3f4f6;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff;'>
        <!-- Header -->
        <div style='background: linear-gradient(135deg, #3b82f6 0%, #8b5cf6 100%); padding: 40px 30px; text-align: center;'>
            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>
                <span style='font-size: 32px;'>🛡️</span> Welcome to Firmeza!
            </h1>
            <p style='color: #e0e7ff; margin: 10px 0 0 0; font-size: 16px;'>Your account is ready</p>
        </div>

        <!-- Content -->
        <div style='padding: 40px 30px;'>
            <h2 style='color: #1f2937; margin: 0 0 20px 0; font-size: 24px;'>Hello {firstName} {lastName}!</h2>
            
            <p style='color: #4b5563; line-height: 1.6; margin: 0 0 30px 0;'>
                Thank you for registering with Firmeza Admin. Your account has been successfully created and you're ready to get started!
            </p>

            <!-- Account Details -->
            <div style='background-color: #f9fafb; border-left: 4px solid #3b82f6; padding: 20px; margin-bottom: 30px; border-radius: 4px;'>
                <h3 style='color: #1f2937; margin: 0 0 15px 0; font-size: 18px;'>Account Details</h3>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 8px 0; color: #6b7280;'>Email:</td>
                        <td style='padding: 8px 0; color: #1f2937; font-weight: 600; text-align: right;'>{email}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 0; color: #6b7280;'>Role:</td>
                        <td style='padding: 8px 0; color: #1f2937; font-weight: 600; text-align: right;'>{role}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 0; color: #6b7280;'>Status:</td>
                        <td style='padding: 8px 0; color: #059669; font-weight: 700; text-align: right;'>Active</td>
                    </tr>
                </table>
            </div>

            <!-- Next Steps -->
            <h3 style='color: #1f2937; margin: 0 0 15px 0; font-size: 18px;'>Next Steps</h3>
            <ul style='color: #4b5563; line-height: 1.8; padding-left: 20px;'>
                <li>Login to your account with your email and password</li>
                <li>Explore our product catalog</li>
                <li>Start shopping and enjoy our services!</li>
            </ul>

            <p style='color: #4b5563; line-height: 1.6; margin: 20px 0 0 0;'>
                If you have any questions or need assistance, please don't hesitate to contact our support team.
            </p>

            <p style='color: #4b5563; line-height: 1.6; margin: 20px 0 0 0;'>
                Best regards,<br>
                <strong>The Firmeza Team</strong>
            </p>
        </div>

        <!-- Footer -->
        <div style='background-color: #f9fafb; padding: 30px; text-align: center; border-top: 1px solid #e5e7eb;'>
            <p style='color: #6b7280; margin: 0; font-size: 14px;'>
                © {DateTime.Now.Year} Firmeza Admin. All rights reserved.
            </p>
            <p style='color: #9ca3af; margin: 10px 0 0 0; font-size: 12px;'>
                This is an automated email. Please do not reply to this message.
            </p>
        </div>
    </div>
</body>
</html>";
    }
}

