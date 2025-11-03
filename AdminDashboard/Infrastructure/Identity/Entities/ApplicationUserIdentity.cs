namespace AdminDashboard.Infrastructure.Identity.Entities;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents the Identity implementation of a user.
/// This class is specific to ASP.NET Identity and EF Core.
/// </summary>
public class ApplicationUserIdentity : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}