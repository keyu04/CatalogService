using CatalogService.DTOs.Inventory;

namespace CatalogService.Services.Interfaces;

public interface IInventoryService
{
    Task<InventoryResponseDto?> GetByProductIdAsync(Guid productId);
    Task<InventoryResponseDto?> UpdateAsync(Guid productId, UpdateInventoryDto dto);
    Task<bool> ReserveStockAsync(Guid productId, int quantity);
    Task<bool> ReleaseStockAsync(Guid productId, int quantity);
}