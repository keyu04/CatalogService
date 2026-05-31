namespace CatalogService.Models;

public class Inventory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }                // ← FK to Product
    public int StockQuantity { get; set; } = 0;
    public int ReservedQuantity { get; set; } = 0;
    public int LowStockThreshold { get; set; } = 5;
    public bool IsInStock { get; set; } = true;
    public DateTime? RestockEta { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // ── Navigation ───────────────────────────────────────────────
    public Product? Product { get; set; }

    // ── Computed — not stored in DB ──────────────────────────────
    public bool IsLowStock => StockQuantity <= LowStockThreshold;
    public int AvailableQuantity => StockQuantity - ReservedQuantity;
}