using CatalogService.DTOs.Inventory;
using CatalogService.Repository.Interfaces;
using CatalogService.Services.Interfaces;

namespace CatalogService.Services.Implementations;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repo;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IInventoryRepository repo, ILogger<InventoryService> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<InventoryResponseDto?> GetByProductIdAsync(Guid productId)
    {
        var inventory = await _repo.GetByProductIdAsync(productId);
        return inventory is null ? null : MapToDto(inventory);
    }

    public async Task<InventoryResponseDto?> UpdateAsync(Guid productId, UpdateInventoryDto dto)
    {
        var inventory = await _repo.GetByProductIdAsync(productId);
        if (inventory is null) return null;

        inventory.StockQuantity     = dto.StockQuantity;
        inventory.LowStockThreshold = dto.LowStockThreshold;
        inventory.RestockEta        = dto.RestockEta;
        inventory.IsInStock         = (dto.StockQuantity - inventory.ReservedQuantity) > 0;

        var updated = await _repo.UpdateAsync(inventory);
        return MapToDto(updated);
    }

    public async Task<bool> ReserveStockAsync(Guid productId, int quantity)
    {
        var result = await _repo.ReserveStockAsync(productId, quantity);

        if (result)
            _logger.LogInformation(
                "Stock reserved | ProductId: {Id} | Quantity: {Qty}", productId, quantity);
        else
            _logger.LogWarning(
                "Stock reservation failed | ProductId: {Id} | Quantity: {Qty}", productId, quantity);

        return result;
    }

    public async Task<bool> ReleaseStockAsync(Guid productId, int quantity)
    {
        var result = await _repo.ReleaseStockAsync(productId, quantity);

        _logger.LogInformation(
            "Stock released | ProductId: {Id} | Quantity: {Qty}", productId, quantity);

        return result;
    }

    private static InventoryResponseDto MapToDto(Models.Inventory i) => new()
    {
        Id                 = i.Id,
        ProductId          = i.ProductId,
        ProductName        = i.Product?.Name ?? string.Empty,
        StockQuantity      = i.StockQuantity,
        ReservedQuantity   = i.ReservedQuantity,
        AvailableQuantity  = i.AvailableQuantity,
        LowStockThreshold  = i.LowStockThreshold,
        IsInStock          = i.IsInStock,
        IsLowStock         = i.IsLowStock,
        RestockEta         = i.RestockEta,
        UpdatedAt          = i.UpdatedAt
    };
}