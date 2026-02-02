using System;
using System.ComponentModel.DataAnnotations;

namespace Andro.Backend.Reference.Products;

public class CreateProductDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}
