using System.ComponentModel.DataAnnotations;

namespace Andro.Backend.Reference.Categories;

public class UpdateCategoryDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; } = null!;

    [StringLength(512)]
    public string? Description { get; set; }
}
