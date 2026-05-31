using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs.Category;

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Category name is required.")]
    [MaxLength(80, ErrorMessage = "Name must not exceed 80 characters.")]
    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }
    public string? Emoji { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; }
}