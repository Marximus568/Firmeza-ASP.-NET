namespace AdminDashboard.Infrastructure.Routing;

public static class ProductRoutes
{
    public static void MapProductRoutes(this WebApplication app)
    {
        app.MapRazorPages();

        app.MapGet("/products", () => Results.Redirect("/Products/Index"));
        app.MapGet("/products/create", () => Results.Redirect("/Products/Create"));
        app.MapGet("/products/edit", () => Results.Redirect("/Products/Edit"));
        app.MapGet("/products/details", () => Results.Redirect("/Products/Details"));
        app.MapGet("/products/delete", () => Results.Redirect("/Products/Delete"));
    }
}