using AdminDashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // ========================
    // TABLES
    // ========================
    public DbSet<Clients> Clients { get; set; }
    public DbSet<Salers> Salers { get; set; }
    public DbSet<Products> Products { get; set; }
    public DbSet<Sales> Sales { get; set; }
    public DbSet<SaleItems> SaleItems { get; set; }

    // ========================
    // CONFIGURATION
    // ========================
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =============================================
        // INHERITANCE MAPPING: TABLE-PER-CONCRETE-TYPE
        // =============================================
        modelBuilder.Entity<Person>().UseTpcMappingStrategy();

        // Exclude Person from direct mapping (no table)
        modelBuilder.Ignore<Person>();

        // ========== CLIENT ==========
        modelBuilder.Entity<Clients>(entity =>
        {
            entity.ToTable("Clients");

            entity.Property(e => e.Phone)
                  .HasMaxLength(15);

            entity.Property(e => e.Address)
                  .HasMaxLength(200);
            
            entity.Property(e => e.role)
                .HasMaxLength(15);
        });

        // ========== SALER ==========
        modelBuilder.Entity<Salers>(entity =>
        {
            entity.ToTable("Salers");

            entity.Property(e => e.Phone)
                  .HasMaxLength(15);

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);
        });

        // ========== SALES ==========
        modelBuilder.Entity<Sales>(entity =>
        {
            entity.ToTable("Sales");

            entity.HasOne(e => e.Clients)
                  .WithMany(c => c.Sales)
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Salers)
                  .WithMany(s => s.Sales)
                  .HasForeignKey(e => e.SalerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
