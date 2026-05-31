namespace CatalogService.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CategoryId { get; set; }           // ← FK to Category
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Unit { get; set; } = string.Empty;   // e.g. "500g", "1L"
    public long PricePaise { get; set; }               // ← always paise
    public long? MrpPaise { get; set; }                // ← original price
    public decimal Rating { get; set; } = 0;
    public int RatingCount { get; set; } = 0;
    public int DeliveryMinutes { get; set; } = 10;
    public string? Tag { get; set; }                   // e.g. "Fresh", "Organic"
    public bool IsFeatured { get; set; } = false;
    public bool IsTopPick { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    // ── Navigation ───────────────────────────────────────────────
    public Category? Category { get; set; }           // ← parent
    public Inventory? Inventory { get; set; }         // ← child
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}