using System;
using System.Collections.Generic;
using Andro.Backend.Reference.Products;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Andro.Backend.Reference.Categories;

public class Category : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; }

    protected Category()
    {
        Name = string.Empty;
        Products = new List<Product>();
    }

    public Category(Guid id, string name, string? description = null)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), 128);
        Description = description;
        Products = new List<Product>();
    }
}
