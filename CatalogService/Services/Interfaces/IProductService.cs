using CatalogService.Common.DTOs;
using CatalogService.DTOs.Product;

namespace CatalogService.Services.Interfaces;

public interface IProductService
{
    Task<PagedResultDto<ProductResponseDto>> GetAllAsync(
        string? search, Guid? categoryId, bool? isFeatured, bool? inStockOnly,
        int page, int pageSize);
    Task<ProductResponseDto?> GetByIdAsync(Guid id);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
    Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto);
    Task<bool> DeleteAsync(Guid id);
}