namespace AdminDashboard.Infrastructure.Routing;

public static class AccountRoutes
{
    public static void MapAccountRoutes(this WebApplication app)
    {
        app.MapRazorPages();

        app.MapGet("/account/login", () => Results.Redirect("/Account/Login"));
        app.MapGet("/account/logout", () => Results.Redirect("/Account/Logout"));
        app.MapGet("/account/register", () => Results.Redirect("/Account/Register"));
        app.MapGet("/account/registerconfirmation", () => Results.Redirect("/Account/RegisterConfirmation"));
        app.MapGet("/account/accessdenied", () => Results.Redirect("/Account/AccessDenied"));
    }
}