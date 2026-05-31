using CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category>     Categories     => Set<Category>();
    public DbSet<Product>      Products       => Set<Product>();
    public DbSet<ProductImage> ProductImages  => Set<ProductImage>();
    public DbSet<Inventory>    Inventories    => Set<Inventory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Category ─────────────────────────────────────────────
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Slug).IsUnique();
            e.HasIndex(x => x.IsActive);
            e.HasIndex(x => x.DeletedAt);
        });

        // ── Product ───────────────────────────────────────────────
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(160).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(180).IsRequired();
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Unit).HasMaxLength(50).IsRequired();
            e.Property(x => x.Rating).HasPrecision(3, 1);
            e.HasIndex(x => x.IsActive);
            e.HasIndex(x => x.IsFeatured);
            e.HasIndex(x => x.DeletedAt);

            // ── One Category has Many Products ───────────────────
            e.HasOne(x => x.Category)
             .WithMany(x => x.Products)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);  // ← don't delete products if category deleted
        });

        // ── ProductImage ──────────────────────────────────────────
        modelBuilder.Entity<ProductImage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ImageUrl).IsRequired();

            // ── One Product has Many Images ───────────────────────
            e.HasOne(x => x.Product)
             .WithMany(x => x.Images)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);   // ← delete images if product deleted
        });

        // ── Inventory ─────────────────────────────────────────────
        modelBuilder.Entity<Inventory>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ProductId).IsUnique();  // ← one inventory per product

            // ── One Product has One Inventory ─────────────────────
            e.HasOne(x => x.Product)
             .WithOne(x => x.Inventory)
             .HasForeignKey<Inventory>(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);   // ← delete inventory if product deleted

            // ── Ignore computed properties — not DB columns ───────
            e.Ignore(x => x.IsLowStock);
            e.Ignore(x => x.AvailableQuantity);
        });
    }
}