namespace AdminDashboard.Domain.ValueObjects;


/// <summary>
/// Represents all available roles in the system.
/// </summary>
public static class UserRole
{
    public const string Admin = "Admin";
    public const string Client = "Client";
    public const string Saler = "Saler";

    // You can add more roles here as needed
    public static readonly string[] AllRoles = { Admin, Client, Saler };
}

