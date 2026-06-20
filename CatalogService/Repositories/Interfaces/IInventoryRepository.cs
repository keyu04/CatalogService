using CatalogService.Models;

namespace CatalogService.Repository.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetByProductIdAsync(Guid productId);
    Task<Inventory> CreateAsync(Inventory inventory);
    Task<Inventory> UpdateAsync(Inventory inventory);
    Task<bool> ReserveStockAsync(Guid productId, int quantity);
    Task<bool> ReleaseStockAsync(Guid productId, int quantity);
}