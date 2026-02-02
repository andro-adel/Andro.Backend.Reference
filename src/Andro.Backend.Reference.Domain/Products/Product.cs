using System;
using Andro.Backend.Reference.Categories;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Andro.Backend.Reference.Products;

public class Product : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }

    public Category? Category { get; set; }

    protected Product()
    {
        Name = string.Empty;
    }

    public Product(
        Guid id,
        string name,
        decimal price,
        int stock,
        Guid categoryId,
        string? description = null) : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), 128);
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        Description = description;
    }
}
