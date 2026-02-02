using System;

namespace Andro.Backend.Reference.Products;

public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ProductCreatedEvent(
        Guid productId,
        string productName,
        decimal price,
        int stock,
        Guid categoryId)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
    }
}
