
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
        
        // =============================================
        // SEED DATA
        // =============================================
        
        // Categories
        modelBuilder.Entity<Categories>().HasData(
            new Categories { Id = 1, Name = "Cemento y Morteros" },
            new Categories { Id = 2, Name = "Ladrillos y Bloques" },
            new Categories { Id = 3, Name = "Arena y Grava" },
            new Categories { Id = 4, Name = "Madera y Derivados" }
        );
        
        // Clients (Users) - All with hashed password "password123"
        // BCrypt hash for "password123" with work factor 12
        var defaultPasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LwFfh.8QW9h4bQ.Rm";
        
        modelBuilder.Entity<Clients>().HasData(
            new Clients
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "System",
                Email = "admin@firmeza.com",
                PhoneNumber = "3001234567",
                Address = "Calle Principal #123, Bogotá",
                Role = "Admin",
                Password = defaultPasswordHash
            },
            new Clients
            {
                Id = 2,
                FirstName = "Juan",
                LastName = "Pérez",
                Email = "juanperez@example.com",
                PhoneNumber = "3109876543",
                Address = "Carrera 10 #45-67, Medellín",
                Role = "Client",
                Password = defaultPasswordHash
            },
            new Clients
            {
                Id = 3,
                FirstName = "María",
                LastName = "Rodríguez",
                Email = "mariarodriguez@example.com",
                PhoneNumber = "3157654321",
                Address = "Avenida 15 #89-12, Cali",
                Role = "Client",
                Password = defaultPasswordHash
            }
        );
        
        // Products
        modelBuilder.Entity<Products>().HasData(
            // Cemento y Morteros
            new Products 
            { 
                Id = 1, 
                Name = "Cemento Argos Portland Gris 50kg", 
                Description = "Cemento de alta resistencia para todo tipo de construcción", 
                UnitPrice = 32000m, 
                Stock = 150, 
                CategoryId = 1, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            },
            new Products 
            { 
                Id = 2, 
                Name = "Mortero Premezclado 40kg", 
                Description = "Mezcla lista para pega de bloques y ladrillos", 
                UnitPrice = 18500m, 
                Stock = 80, 
                CategoryId = 1, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            },
            
            // Ladrillos y Bloques
            new Products 
            { 
                Id = 3, 
                Name = "Ladrillo Tolete Macizo", 
                Description = "Ladrillo rojo tradicional de arcilla cocida", 
                UnitPrice = 950m, 
                Stock = 5000, 
                CategoryId = 2, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            },
            new Products 
            { 
                Id = 4, 
                Name = "Bloque #4 (10x20x40cm)", 
                Description = "Bloque de concreto estructural estándar", 
                UnitPrice = 2100m, 
                Stock = 2000, 
                CategoryId = 2, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            },
            new Products 
            { 
                Id = 5, 
                Name = "Bloque #5 (12x20x40cm)", 
                Description = "Bloque de concreto para muros de carga", 
                UnitPrice = 2400m, 
                Stock = 1500, 
                CategoryId = 2, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            },
            
            // Arena y Grava
            new Products 
            { 
                Id = 6, 
                Name = "Arena Lavada - Bulto 40kg", 
                Description = "Arena fina lavada para mezclas y acabados", 
                UnitPrice = 8500m, 
                Stock = 300, 
                CategoryId = 3, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            },
            new Products 
            { 
                Id = 7, 
                Name = "Gravilla #67 - Bulto 40kg", 
                Description = "Agregado grueso para concreto estructural", 
                UnitPrice = 9200m, 
                Stock = 250, 
                CategoryId = 3, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            },
            
            // Madera y Derivados
            new Products 
            { 
                Id = 8, 
                Name = "Tabla Triplex 6mm 1.22x2.44m", 
                Description = "Placa de triplex para formaletas y acabados", 
                UnitPrice = 45000m, 
                Stock = 120, 
                CategoryId = 4, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            },
            new Products 
            { 
                Id = 9, 
                Name = "Regla Pino 2x4x3m", 
                Description = "Madera de pino cepillada para estructura", 
                UnitPrice = 15800m, 
                Stock = 90, 
                CategoryId = 4, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            },
            new Products 
            { 
                Id = 10, 
                Name = "Tabla Burra 1x12x3m", 
                Description = "Madera bruta para formaleta y obra negra", 
                UnitPrice = 28000m, 
                Stock = 75, 
                CategoryId = 4, 
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) 
            }
        );
        
        base.OnModelCreating(modelBuilder);
    }
}
