using System;
using System.ComponentModel.DataAnnotations;
using Andro.Backend.Reference.Products;

namespace Andro.Backend.Reference.Products;

public class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(
        ProductConsts.MaxNameLength,
        MinimumLength = ProductConsts.MinNameLength,
        ErrorMessage = "Product name must be between {2} and {1} characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Price is required")]
    [Range(
        typeof(decimal),
        "0.01",
        "1000000",
        ErrorMessage = "Price must be between {1} and {2}")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(
        ProductConsts.MinStock,
        ProductConsts.MaxStock,
        ErrorMessage = "Stock must be between {1} and {2}")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    [StringLength(
        ProductConsts.MaxDescriptionLength,
        ErrorMessage = "Description cannot exceed {1} characters")]
    public string? Description { get; set; }
}
