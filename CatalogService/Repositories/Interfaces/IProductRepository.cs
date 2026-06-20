using CatalogService.Common.DTOs;
using CatalogService.Models;

namespace CatalogService.Repository.Interfaces;

public interface IProductRepository
{
    Task<PagedResultDto<Product>> GetAllAsync(
        string? search, Guid? categoryId, bool? isFeatured, bool? inStockOnly,
        int page, int pageSize);
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetBySlugAsync(string slug);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsBySlugAsync(string slug);
}