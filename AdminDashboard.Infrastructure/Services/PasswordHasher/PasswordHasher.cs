using AdminDashboardApplication.Services;
using BCrypt.Net;

namespace AdminDashboard.Infrastructure.Services.PasswordHasher;

/// <summary>
/// Implementation of password hashing service using BCrypt
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    // BCrypt work factor - higher is more secure but slower
    // 12 is a good balance between security and performance
    private const int WorkFactor = 12;

    /// <summary>
    /// Hashes a plain-text password using BCrypt with work factor 12
    /// </summary>
    /// <param name="password">The plain-text password to hash</param>
    /// <returns>The hashed password</returns>
    /// <exception cref="ArgumentNullException">Thrown when password is null or empty</exception>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Verifies a plain-text password against a hashed password
    /// </summary>
    /// <param name="password">The plain-text password to verify</param>
    /// <param name="hash">The hashed password to verify against</param>
    /// <returns>True if the password matches the hash, false otherwise</returns>
    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            // If hash is invalid or verification fails, return false
            return false;
        }
    }
}
