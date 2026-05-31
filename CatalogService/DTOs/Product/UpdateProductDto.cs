using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs.Product;

public class UpdateProductDto
{
    [Required(ErrorMessage = "Category is required.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [MaxLength(160, ErrorMessage = "Name must not exceed 160 characters.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty;

    [Range(1, long.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public long PricePaise { get; set; }

    public long? MrpPaise { get; set; }
    public int DeliveryMinutes { get; set; } = 10;
    public string? Tag { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsTopPick { get; set; }
    public bool IsActive { get; set; }
}