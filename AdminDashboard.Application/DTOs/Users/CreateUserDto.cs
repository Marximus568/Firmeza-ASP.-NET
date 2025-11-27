using System.ComponentModel.DataAnnotations;

namespace AdminDashboardApplication.DTOs.Users;

public class CreateUserDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    public DateOnly DateOfBirth { get; set; }
    
    [Phone]
    [StringLength(15, MinimumLength = 7)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [StringLength(200, MinimumLength = 5)]
    public string Address { get; set; } = string.Empty;
    
    [StringLength(200, MinimumLength = 5)]
    public string Password { get; set; } = string.Empty;
    
    

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "Client";
}