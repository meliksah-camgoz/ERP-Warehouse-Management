
using Microsoft.EntityFrameworkCore;
using MiniERP.Domain.Entities;

namespace MiniERP.Infrastructure.Persistence;

public class MiniERPDbContext : DbContext
{
    public MiniERPDbContext(
        DbContextOptions<MiniERPDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    public DbSet<StockBalance> StockBalances => Set<StockBalance>();

    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<AppUser> AppUsers => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.SKU)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(x => x.SKU).IsUnique();

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.PurchasePrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.SalePrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.VatRate)
                .HasPrecision(5, 2);

            entity.Property(x => x.MinStockLevel)
                .HasPrecision(18, 3);

            entity.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.Property(x => x.Code)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(x => x.Code).IsUnique();

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();
        });

        modelBuilder.Entity<StockBalance>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.Quantity)
                .HasPrecision(18, 2);

            entity.HasIndex(x => new { x.ProductId, x.WarehouseId })
                .IsUnique();
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.MovementType)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.Quantity)
                .HasPrecision(18, 2);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.CreatedAtUtc);
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.AssetNumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(x => x.AssetNumber)
                .IsUnique();

            entity.Property(x => x.AssetType)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Brand)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Model)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.SerialNumber)
                .HasMaxLength(100);

            entity.Property(x => x.AssignedTo)
                .HasMaxLength(150);

            entity.Property(x => x.Department)
                .HasMaxLength(100);

            entity.Property(x => x.Location)
                .HasMaxLength(200);

            entity.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(1000);
        });
    }
}
