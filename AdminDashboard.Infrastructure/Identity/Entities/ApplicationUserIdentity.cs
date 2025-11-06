using Microsoft.AspNetCore.Identity;

// Duplicate ApplicationUserIdentity removed in Infrastructure.
// Use AdminDashboard.Identity.Entities.ApplicationUserIdentity (the canonical type) instead.

namespace AdminDashboard.Infrastructure.Identity.Entities;

public class ApplicationUserIdentity : AdminDashboard.Identity.Entities.ApplicationUserIdentity
{
}