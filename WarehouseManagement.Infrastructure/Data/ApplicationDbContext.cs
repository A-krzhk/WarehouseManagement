using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<InventoryOperation> InventoryOperations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Location>()
            .HasOne(l => l.Warehouse)
            .WithMany();
        
        builder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InventoryOperation>()
            .HasOne(io => io.Product)
            .WithMany(p => p.InventoryOperations)
            .HasForeignKey(io => io.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InventoryOperation>()
            .HasOne(io => io.User)
            .WithMany(u => u.InventoryOperations)
            .HasForeignKey(io => io.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Location>()
            .HasOne(l => l.Warehouse)
            .WithMany(w => w.Locations)
            .HasForeignKey(l => l.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProductLocation>()
            .HasOne(pl => pl.Product)
            .WithMany(p => p.ProductLocations)
            .HasForeignKey(pl => pl.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProductLocation>()
            .HasOne(pl => pl.Location)
            .WithMany(l => l.ProductLocations)
            .HasForeignKey(pl => pl.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}