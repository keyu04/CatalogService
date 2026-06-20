using CatalogService.Common.DTOs;
using CatalogService.Models;

namespace CatalogService.Repository.Interfaces;

public interface ICategoryRepository
{
    Task<PagedResultDto<Category>> GetAllAsync(string? search, int page, int pageSize);
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetBySlugAsync(string slug);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsBySlugAsync(string slug);
    Task<bool> ExistsByIdAsync(Guid id);
}