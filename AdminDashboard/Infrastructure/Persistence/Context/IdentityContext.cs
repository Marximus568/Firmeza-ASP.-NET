namespace AdminDashboard.Infrastructure.Data;

using AdminDashboard.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class IdentityContext : IdentityDbContext<ApplicationUserIdentity>
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUserIdentity>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
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