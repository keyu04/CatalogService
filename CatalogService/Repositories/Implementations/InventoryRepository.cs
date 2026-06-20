using CatalogService.Data;
using CatalogService.Models;
using CatalogService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Repository.Implementations;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _db;

    public InventoryRepository(AppDbContext db) => _db = db;

    public async Task<Inventory?> GetByProductIdAsync(Guid productId) =>
        await _db.Inventories
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.ProductId == productId);

    public async Task<Inventory> CreateAsync(Inventory inventory)
    {
        _db.Inventories.Add(inventory);
        await _db.SaveChangesAsync();
        return inventory;
    }

    public async Task<Inventory> UpdateAsync(Inventory inventory)
    {
        inventory.UpdatedAt = DateTime.UtcNow;
        _db.Inventories.Update(inventory);
        await _db.SaveChangesAsync();
        return inventory;
    }

    // ── LINQ Lesson 8 — Concurrency safe stock operations ─────────
    public async Task<bool> ReserveStockAsync(Guid productId, int quantity)
    {
        var inventory = await _db.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (inventory is null) return false;

        // ── Check available stock before reserving ────────────────
        var available = inventory.StockQuantity - inventory.ReservedQuantity;
        if (available < quantity) return false;

        inventory.ReservedQuantity += quantity;

        // ── Update IsInStock flag automatically ───────────────────
        inventory.IsInStock = (inventory.StockQuantity - inventory.ReservedQuantity) > 0;
        inventory.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReleaseStockAsync(Guid productId, int quantity)
    {
        var inventory = await _db.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (inventory is null) return false;

        // ── Never go below zero ───────────────────────────────────
        inventory.ReservedQuantity = Math.Max(0, inventory.ReservedQuantity - quantity);
        inventory.IsInStock = (inventory.StockQuantity - inventory.ReservedQuantity) > 0;
        inventory.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }
}