using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Andro.Backend.Reference.Products;

public class Product : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
    
    public string Description { get; set; }
    
    protected Product()
    {
    }
    
    public Product(
        Guid id,
        string name,
        decimal price,
        int stock = 0,
        string description = null) : base(id)
    {
        Name = name;
        Price = price;
        Stock = stock;
        Description = description;
    }
}
