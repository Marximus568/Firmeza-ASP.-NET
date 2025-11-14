namespace AdminDashboard.Domain.Models;

public abstract class Person
{
    // Primary key managed by the database
    public int Id { get; set; }

    // Common attributes
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
}
