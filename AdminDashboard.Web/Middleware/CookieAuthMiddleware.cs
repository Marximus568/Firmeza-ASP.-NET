using AdminDashboard.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace AdminDashboard.Middleware;

/// <summary>
/// Middleware to automatically authenticate users from JWT cookie
/// Enables SSO between React front-end and Admin Dashboard
/// </summary>
public class CookieAuthMiddleware
{
    private readonly RequestDelegate _next;

    public CookieAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        SignInManager<ApplicationUserIdentity> signInManager,
        UserManager<ApplicationUserIdentity> userManager)
    {
        // Only process if user is not already authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            // Try to read JWT token from cookie
            if (context.Request.Cookies.TryGetValue("auth_token", out var token) && !string.IsNullOrEmpty(token))
            {
                try
                {
                    // For simplicity, we'll extract the email from the token without full JWT validation
                    // In production, you should validate the JWT properly
                    var payload = ExtractEmailFromToken(token);
                    
                    if (!string.IsNullOrEmpty(payload))
                    {
                        // Find user by email
                        var user = await userManager.FindByEmailAsync(payload);
                        if (user != null)
                        {
                            // Sign in the user automatically (creates ASP.NET Core session)
                            await signInManager.SignInAsync(user, isPersistent: true);
                            
                            // Remove the cookie after successful authentication
                            context.Response.Cookies.Delete("auth_token");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error but don't block the request
                    Console.WriteLine($"[CookieAuthMiddleware] Error: {ex.Message}");
                    // Remove invalid cookie
                    context.Response.Cookies.Delete("auth_token");
                }
            }
        }

        await _next(context);
    }

    /// <summary>
    /// Simple JWT payload extraction (email claim)
    /// Note: This is a simplified version. In production, use proper JWT validation.
    /// </summary>
    private string? ExtractEmailFromToken(string token)
    {
        try
        {
            // JWT format: header.payload.signature
            var parts = token.Split('.');
            if (parts.Length != 3) return null;

            // Decode base64 payload
            var payload = parts[1];
            // Add padding if needed
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);

            // Parse JSON to extract email
            // Simple parsing - in production use System.Text.Json
            var emailIndex = payloadJson.IndexOf("\"email\":\"");
            if (emailIndex == -1) return null;

            var emailStart = emailIndex + 9; // Length of "\"email\":\""
            var emailEnd = payloadJson.IndexOf("\"", emailStart);
            if (emailEnd == -1) return null;

            return payloadJson.Substring(emailStart, emailEnd - emailStart);
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// Extension method to register the middleware
/// </summary>
public static class CookieAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseCookieAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CookieAuthMiddleware>();
    }
}
