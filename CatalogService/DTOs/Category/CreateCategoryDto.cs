using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs.Category;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Category name is required.")]
    [MaxLength(80, ErrorMessage = "Name must not exceed 80 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required.")]
    [MaxLength(100, ErrorMessage = "Slug must not exceed 100 characters.")]
    [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$",
        ErrorMessage = "Slug must be lowercase letters, numbers and hyphens only.")]
    public string Slug { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }
    public string? Emoji { get; set; }
    public int SortOrder { get; set; } = 0;
}