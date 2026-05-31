namespace CatalogService.DTOs.Inventory;

public class InventoryResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;   // ← flattened
    public int StockQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }                // ← computed
    public int LowStockThreshold { get; set; }
    public bool IsInStock { get; set; }
    public bool IsLowStock { get; set; }                      // ← computed
    public DateTime? RestockEta { get; set; }
    public DateTime UpdatedAt { get; set; }
}