using AdminDashboard.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Identity.Persistence.Context;

public class IdentityContext : IdentityDbContext<ApplicationUserIdentity>
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Use distinct table names for Identity to avoid collisions with domain tables
        builder.Entity<ApplicationUserIdentity>().ToTable("IdentityUsers");
        builder.Entity<IdentityRole>().ToTable("IdentityRoles");
        builder.Entity<IdentityUserRole<string>>().ToTable("IdentityUserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("IdentityUserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("IdentityUserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("IdentityRoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("IdentityUserTokens");

        builder.Entity<ApplicationUserIdentity>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.Entity<ApplicationUserIdentity>()
            .Property(u => u.UpdatedAt)
            .IsRequired(false);

        builder.Entity<ApplicationUserIdentity>()
            .Property(u => u.IsActive)
            .HasDefaultValue(true);
    }
}
