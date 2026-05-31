using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs.Product;

public class CreateProductDto
{
    [Required(ErrorMessage = "Category is required.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [MaxLength(160, ErrorMessage = "Name must not exceed 160 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required.")]
    [MaxLength(180, ErrorMessage = "Slug must not exceed 180 characters.")]
    [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$",
        ErrorMessage = "Slug must be lowercase letters, numbers and hyphens only.")]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    [MaxLength(50, ErrorMessage = "Unit must not exceed 50 characters.")]
    public string Unit { get; set; } = string.Empty;

    [Range(1, long.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public long PricePaise { get; set; }

    public long? MrpPaise { get; set; }
    public int DeliveryMinutes { get; set; } = 10;
    public string? Tag { get; set; }
    public bool IsFeatured { get; set; } = false;
    public bool IsTopPick { get; set; } = false;

    // ── Initial stock when product is created ────────────────────
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater.")]
    public int InitialStock { get; set; } = 0;
}