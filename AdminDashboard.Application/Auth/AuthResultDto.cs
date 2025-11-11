namespace AdminDashboardApplication.Auth;


public record AuthResultDto
{
    public bool Succeeded { get; init; }
    public string? UserId { get; init; }
    public string? Email { get; init; }
    public IEnumerable<string> Roles { get; init; } = Array.Empty<string>();
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Creates a successful authentication result
    /// </summary>
    public static AuthResultDto Success(string userId, string email, IEnumerable<string> roles)
        => new()
        {
            Succeeded = true,
            UserId = userId,
            Email = email,
            Roles = roles
        };

    /// <summary>
    /// Creates a failed authentication result
    /// </summary>
    public static AuthResultDto Failure(IEnumerable<string> errors)
        => new()
        {
            Succeeded = false,
            Errors = errors
        };

    /// <summary>
    /// Creates a failed authentication result with a single error
    /// </summary>
    public static AuthResultDto Failure(string error)
        => Failure(new[] { error });
}