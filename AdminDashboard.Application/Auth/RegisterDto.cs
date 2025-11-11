using System.ComponentModel.DataAnnotations;

namespace AdminDashboardApplication.Auth;

/// <summary>
/// Data transfer object for user registration
/// </summary>
public record RegisterDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50)]
    public string FirstName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(50)]
    public string LastName { get; init; } = string.Empty;

    [MaxLength(15)]
    public string? PhoneNumber { get; init; }

    [MaxLength(200)]
    public string? Address { get; init; }

    /// <summary>
    /// Optional role assignment during registration (defaults to Client)
    /// </summary>
    public string? Role { get; init; }
}