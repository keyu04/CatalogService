namespace CatalogService.DTOs.Product;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;   // ← flattened
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Unit { get; set; } = string.Empty;
    public long PricePaise { get; set; }
    public long? MrpPaise { get; set; }
    public decimal Rating { get; set; }
    public int RatingCount { get; set; }
    public int DeliveryMinutes { get; set; }
    public string? Tag { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsTopPick { get; set; }
    public bool IsActive { get; set; }

    // ── Inventory info flattened ──────────────────────────────────
    public bool IsInStock { get; set; }
    public int StockQuantity { get; set; }
    public bool IsLowStock { get; set; }

    // ── Images ───────────────────────────────────────────────────
    public List<string> ImageUrls { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}