namespace AdminDashboard.Infrastructure.Routing;

public static class RoutingExtensions
{
    public static void MapAppRoutes(this WebApplication app)
    {
        app.MapAccountRoutes();
        app.MapAdminRoutes();
        app.MapProductRoutes();
        app.MapUserRoutes();
    }
}