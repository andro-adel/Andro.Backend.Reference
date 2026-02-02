using System.ComponentModel.DataAnnotations;
using Andro.Backend.Reference.Categories;

namespace Andro.Backend.Reference.Categories;

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(
        CategoryConsts.MaxNameLength,
        MinimumLength = CategoryConsts.MinNameLength,
        ErrorMessage = "Category name must be between {2} and {1} characters")]
    public string Name { get; set; } = null!;

    [StringLength(
        CategoryConsts.MaxDescriptionLength,
        ErrorMessage = "Description cannot exceed {1} characters")]
    public string? Description { get; set; }
}
