using CatalogService.Common.DTOs;
using CatalogService.Data;
using CatalogService.Models;
using CatalogService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Repository.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db) => _db = db;

    public async Task<PagedResultDto<Category>> GetAllAsync(
        string? search, int page, int pageSize)
    {
        // ── LINQ Lesson 1 — Building queries dynamically ──────────
        var query = _db.Categories
            .Where(c => c.DeletedAt == null)   // ← soft delete filter
            .AsQueryable();

        // ── LINQ Lesson 2 — Conditional filtering ─────────────────
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search));

        var total = await query.CountAsync();

        // ── LINQ Lesson 3 — Projection with Select ────────────────
        // We don't load Products navigation — we just COUNT them
        var items = await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<Category>
        {
            Items      = items,
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        };
    }

    public async Task<Category?> GetByIdAsync(Guid id) =>
        await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

    public async Task<Category?> GetBySlugAsync(string slug) =>
        await _db.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug && c.DeletedAt == null);

    public async Task<Category> CreateAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        _db.Categories.Update(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null || category.DeletedAt != null) return false;

        category.DeletedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsBySlugAsync(string slug) =>
        await _db.Categories
            .AnyAsync(c => c.Slug == slug && c.DeletedAt == null);

    public async Task<bool> ExistsByIdAsync(Guid id) =>
        await _db.Categories
            .AnyAsync(c => c.Id == id && c.DeletedAt == null);
}