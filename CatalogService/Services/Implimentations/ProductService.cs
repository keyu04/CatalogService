using CatalogService.Common.DTOs;
using CatalogService.Data;
using CatalogService.DTOs.Product;
using CatalogService.Models;
using CatalogService.Repository.Interfaces;
using CatalogService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository  _repo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly AppDbContext        _db;          // ← needed for transaction
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository repo,
        ICategoryRepository categoryRepo,
        AppDbContext db,
        ILogger<ProductService> logger)
    {
        _repo         = repo;
        _categoryRepo = categoryRepo;
        _db           = db;
        _logger       = logger;
    }

    public async Task<PagedResultDto<ProductResponseDto>> GetAllAsync(
        string? search, Guid? categoryId, bool? isFeatured, bool? inStockOnly,
        int page, int pageSize)
    {
        var result = await _repo.GetAllAsync(
            search, categoryId, isFeatured, inStockOnly, page, pageSize);

        return new PagedResultDto<ProductResponseDto>
        {
            Items      = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page       = result.Page,
            PageSize   = result.PageSize
        };
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product is null ? null : MapToDto(product);
    }

    // ── EF CORE TRANSACTION LESSON STARTS HERE ────────────────────
    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        // ── Validate category exists BEFORE starting transaction ──
        var categoryExists = await _categoryRepo.ExistsByIdAsync(dto.CategoryId);
        if (!categoryExists)
            throw new KeyNotFoundException($"Category with id '{dto.CategoryId}' not found.");

        if (await _repo.ExistsBySlugAsync(dto.Slug))
            throw new InvalidOperationException($"Slug '{dto.Slug}' already exists.");

        // ── Start transaction — both operations succeed or both fail ──
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var product = new Product
            {
                CategoryId      = dto.CategoryId,
                Name            = dto.Name.Trim(),
                Slug            = dto.Slug.Trim().ToLower(),
                Description     = dto.Description,
                ImageUrl        = dto.ImageUrl,
                Unit            = dto.Unit,
                PricePaise      = dto.PricePaise,
                MrpPaise        = dto.MrpPaise,
                DeliveryMinutes = dto.DeliveryMinutes,
                Tag             = dto.Tag,
                IsFeatured      = dto.IsFeatured,
                IsTopPick       = dto.IsTopPick
            };

            // ── Step 1 — Create Product ────────────────────────────
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            // ── Step 2 — Create Inventory for that Product ─────────
            var inventory = new Inventory
            {
                ProductId     = product.Id,
                StockQuantity = dto.InitialStock,
                IsInStock     = dto.InitialStock > 0
            };

            _db.Inventories.Add(inventory);
            await _db.SaveChangesAsync();

            // ── Both succeeded — commit transaction ─────────────────
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Product created with inventory | ProductId: {Id} | Stock: {Stock}",
                product.Id, dto.InitialStock);

            // ── Reload with all navigation properties for response ──
            var created = await _repo.GetByIdAsync(product.Id);
            return MapToDto(created!);
        }
        catch (Exception ex)
        {
            // ── Something failed — undo everything ──────────────────
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to create product, transaction rolled back");
            throw;
        }
    }
    // ── EF CORE TRANSACTION LESSON ENDS HERE ──────────────────────

    public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product is null) return null;

        var categoryExists = await _categoryRepo.ExistsByIdAsync(dto.CategoryId);
        if (!categoryExists)
            throw new KeyNotFoundException($"Category with id '{dto.CategoryId}' not found.");

        product.CategoryId      = dto.CategoryId;
        product.Name            = dto.Name.Trim();
        product.Description     = dto.Description;
        product.ImageUrl        = dto.ImageUrl;
        product.Unit            = dto.Unit;
        product.PricePaise      = dto.PricePaise;
        product.MrpPaise        = dto.MrpPaise;
        product.DeliveryMinutes = dto.DeliveryMinutes;
        product.Tag             = dto.Tag;
        product.IsFeatured      = dto.IsFeatured;
        product.IsTopPick       = dto.IsTopPick;
        product.IsActive        = dto.IsActive;

        var updated = await _repo.UpdateAsync(product);

        var withRelations = await _repo.GetByIdAsync(updated.Id);
        return MapToDto(withRelations!);
    }

    public async Task<bool> DeleteAsync(Guid id) =>
        await _repo.DeleteAsync(id);

    // ── DTO Mapper — flattening navigation properties ─────────────
    private static ProductResponseDto MapToDto(Product p) => new()
    {
        Id              = p.Id,
        CategoryId      = p.CategoryId,
        CategoryName    = p.Category?.Name ?? string.Empty,
        Name            = p.Name,
        Slug            = p.Slug,
        Description     = p.Description,
        ImageUrl        = p.ImageUrl,
        Unit            = p.Unit,
        PricePaise      = p.PricePaise,
        MrpPaise        = p.MrpPaise,
        Rating          = p.Rating,
        RatingCount     = p.RatingCount,
        DeliveryMinutes = p.DeliveryMinutes,
        Tag             = p.Tag,
        IsFeatured      = p.IsFeatured,
        IsTopPick       = p.IsTopPick,
        IsActive        = p.IsActive,
        IsInStock       = p.Inventory?.IsInStock ?? false,
        StockQuantity   = p.Inventory?.StockQuantity ?? 0,
        IsLowStock      = p.Inventory?.IsLowStock ?? false,
        ImageUrls       = p.Images?.Select(i => i.ImageUrl).ToList() ?? new(),
        CreatedAt       = p.CreatedAt
    };
}