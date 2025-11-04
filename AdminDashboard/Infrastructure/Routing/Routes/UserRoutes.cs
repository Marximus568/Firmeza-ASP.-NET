namespace AdminDashboard.Infrastructure.Routing;

public static class UserRoutes
{
    public static void MapUserRoutes(this WebApplication app)
    {
        app.MapRazorPages();

        app.MapGet("/users", () => Results.Redirect("/Shared/Users/Index"));
        app.MapGet("/users/create", () => Results.Redirect("/Shared/Users/Create"));
        app.MapGet("/users/edit", () => Results.Redirect("/Shared/Users/Edit"));
        app.MapGet("/users/details", () => Results.Redirect("/Shared/Users/Details"));
        app.MapGet("/users/delete", () => Results.Redirect("/Shared/Users/Delete"));
    }
}