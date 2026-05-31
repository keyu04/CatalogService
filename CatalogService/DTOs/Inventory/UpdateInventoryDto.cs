using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs.Inventory;

public class UpdateInventoryDto
{
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater.")]
    public int StockQuantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold must be 0 or greater.")]
    public int LowStockThreshold { get; set; } = 5;

    public DateTime? RestockEta { get; set; }
}