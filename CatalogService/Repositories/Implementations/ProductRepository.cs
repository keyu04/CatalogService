using CatalogService.Common.DTOs;
using CatalogService.Data;
using CatalogService.Models;
using CatalogService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Repository.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) => _db = db;

    public async Task<PagedResultDto<Product>> GetAllAsync(
        string? search, Guid? categoryId, bool? isFeatured, bool? inStockOnly,
        int page, int pageSize)
    {
        // ── LINQ Lesson 4 — Multiple Includes (eager loading) ─────
        var query = _db.Products
            .Include(p => p.Category)             // ← load category name
            .Include(p => p.Inventory)            // ← load stock info
            .Include(p => p.Images
                .OrderBy(i => i.SortOrder))       // ← load images sorted
            .Where(p => p.DeletedAt == null)
            .AsQueryable();

        // ── LINQ Lesson 5 — Multiple conditional filters ──────────
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description!.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (isFeatured.HasValue)
            query = query.Where(p => p.IsFeatured == isFeatured.Value);

        // ── LINQ Lesson 6 — Filtering on navigation property ──────
        if (inStockOnly == true)
            query = query.Where(p =>
                p.Inventory != null &&
                p.Inventory.IsInStock);

        var total = await query.CountAsync();

        // ── LINQ Lesson 7 — Multi-column ordering ─────────────────
        var items = await query
            .OrderByDescending(p => p.IsFeatured)  // ← featured first
            .ThenByDescending(p => p.IsTopPick)    // ← then top picks
            .ThenBy(p => p.Name)                   // ← then alphabetical
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<Product>
        {
            Items      = items,
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        };
    }

    public async Task<Product?> GetByIdAsync(Guid id) =>
        await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

    public async Task<Product?> GetBySlugAsync(string slug) =>
        await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(p => p.Slug == slug && p.DeletedAt == null);

    public async Task<Product> CreateAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null || product.DeletedAt != null) return false;

        product.DeletedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsBySlugAsync(string slug) =>
        await _db.Products
            .AnyAsync(p => p.Slug == slug && p.DeletedAt == null);
}