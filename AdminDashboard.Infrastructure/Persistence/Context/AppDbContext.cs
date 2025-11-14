
using AdminDashboard.Domain.Entities;
using AdminDashboard.Domain.Models;
using AdminDashboardApplication.DTOs.Users;
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
    public DbSet<Clients> Users { get; set; }
    public DbSet<Products> Products { get; set; }
    public DbSet<Sales> Sales { get; set; }
    public DbSet<SaleItems> SaleItems { get; set; }
    
    public DbSet<Categories> Categories { get; set; }
    

    // ========================
    // CONFIGURATION
    // ========================
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // =============================================
        // INHERITANCE MAPPING: TABLE-PER-CONCRETE-TYPE
        // =============================================
        modelBuilder.Entity<Person>().UseTpcMappingStrategy();

        // Exclude Person from direct mapping (no table)
        modelBuilder.Ignore<Person>();

        // ========== USERS ==========
        modelBuilder.Entity<Clients>(entity =>
        {
            entity.ToTable("Users");

            entity.Property(e => e.PhoneNumber)
                  .HasMaxLength(15);

            entity.Property(e => e.Address)
                  .HasMaxLength(200);
            
            entity.Property(e => e.Role)
                .HasMaxLength(15);
        });
        

        // ========== SALES ==========
        modelBuilder.Entity<Sales>(entity =>
        {
            entity.ToTable("Sales");

            entity.HasOne(e => e.Clients)
                  .WithMany(c => c.Sales)
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        //============= Products =========
        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasKey(e => e.Id);
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.Description)
                .HasMaxLength(300);

            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(e => e.Stock)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // =========== Categories ==============
        modelBuilder.Entity<Categories>(entity =>
        {
            entity.HasKey(e => e.Id);
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });
        
        // Seed data
        modelBuilder.Entity<Categories>().HasData(
            new Categories { Id = 1, Name = "Cemento y Morteros" },
            new Categories { Id = 2, Name = "Ladrillos y Bloques" },
            new Categories { Id = 3, Name = "Arena y Grava" },
            new Categories { Id = 4, Name = "Madera y Derivados" }
        );
        base.OnModelCreating(modelBuilder);
    }
}
