using Microsoft.EntityFrameworkCore;
using AdminDashboard.Domain.Entities;

namespace AdminDashboard.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ========================
    // TABLES
    // ========================
    public DbSet<Client> Clients { get; set; }
    public DbSet<Saler> Salers { get; set; }
    public DbSet<Sales> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }

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
        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");

            entity.Property(e => e.Phone)
                  .HasMaxLength(15);

            entity.Property(e => e.Address)
                  .HasMaxLength(200);
        });

        // ========== SALER ==========
        modelBuilder.Entity<Saler>(entity =>
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

            entity.HasOne(e => e.Client)
                  .WithMany(c => c.Sales)
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Saler)
                  .WithMany(s => s.Sales)
                  .HasForeignKey(e => e.SalerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
