using CatalogService.Common.DTOs;
using CatalogService.DTOs.Category;
using CatalogService.Models;
using CatalogService.Repository.Interfaces;
using CatalogService.Services.Interfaces;

namespace CatalogService.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository repo, ILogger<CategoryService> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<PagedResultDto<CategoryResponseDto>> GetAllAsync(
        string? search, int page, int pageSize)
    {
        var result = await _repo.GetAllAsync(search, page, pageSize);

        return new PagedResultDto<CategoryResponseDto>
        {
            Items      = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page       = result.Page,
            PageSize   = result.PageSize
        };
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(Guid id)
    {
        var category = await _repo.GetByIdAsync(id);
        return category is null ? null : MapToDto(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
    {
        if (await _repo.ExistsBySlugAsync(dto.Slug))
            throw new InvalidOperationException($"Slug '{dto.Slug}' already exists.");

        var category = new Category
        {
            Name      = dto.Name.Trim(),
            Slug      = dto.Slug.Trim().ToLower(),
            ImageUrl  = dto.ImageUrl,
            Emoji     = dto.Emoji,
            SortOrder = dto.SortOrder
        };

        var created = await _repo.CreateAsync(category);
        _logger.LogInformation("Category created | Id: {Id}", created.Id);

        return MapToDto(created);
    }

    public async Task<CategoryResponseDto?> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null) return null;

        category.Name      = dto.Name.Trim();
        category.ImageUrl  = dto.ImageUrl;
        category.Emoji     = dto.Emoji;
        category.SortOrder = dto.SortOrder;
        category.IsActive  = dto.IsActive;

        var updated = await _repo.UpdateAsync(category);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id) =>
        await _repo.DeleteAsync(id);

    private static CategoryResponseDto MapToDto(Category c) => new()
    {
        Id           = c.Id,
        Name         = c.Name,
        Slug         = c.Slug,
        ImageUrl     = c.ImageUrl,
        Emoji        = c.Emoji,
        SortOrder    = c.SortOrder,
        IsActive     = c.IsActive,
        ProductCount = c.Products?.Count(p => p.DeletedAt == null) ?? 0,
        CreatedAt    = c.CreatedAt
    };
}