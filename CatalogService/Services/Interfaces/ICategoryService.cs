using CatalogService.Common.DTOs;
using CatalogService.DTOs.Category;

namespace CatalogService.Services.Interfaces;

public interface ICategoryService
{
    Task<PagedResultDto<CategoryResponseDto>> GetAllAsync(string? search, int page, int pageSize);
    Task<CategoryResponseDto?> GetByIdAsync(Guid id);
    Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryResponseDto?> UpdateAsync(Guid id, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(Guid id);
}