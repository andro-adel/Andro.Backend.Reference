using System;
using Andro.Backend.Reference.Categories;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Andro.Backend.Reference.Products;

public class Product : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

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
        SetName(name);
        SetPrice(price);
        SetStock(stock);
        CategoryId = categoryId;
        Description = description;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name,
            nameof(name),
            ProductConsts.MaxNameLength
        );
    }

    public void SetPrice(decimal price)
    {
        if (price < ProductConsts.MinPrice || price > ProductConsts.MaxPrice)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.InvalidProductPrice)
                .WithData("Price", price)
                .WithData("MinPrice", ProductConsts.MinPrice)
                .WithData("MaxPrice", ProductConsts.MaxPrice);
        }

        Price = price;
    }

    public void SetStock(int stock)
    {
        if (stock < ProductConsts.MinStock || stock > ProductConsts.MaxStock)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.InvalidProductStock)
                .WithData("Stock", stock)
                .WithData("MinStock", ProductConsts.MinStock)
                .WithData("MaxStock", ProductConsts.MaxStock);
        }

        Stock = stock;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.InvalidProductStock)
                .WithData("Quantity", quantity);
        }

        if (Stock < quantity)
        {
            throw new InsufficientStockException(Name, quantity, Stock);
        }

        var oldStock = Stock;
        Stock -= quantity;

        AddLocalEvent(new ProductStockChangedEvent(
            Id,
            Name,
            oldStock,
            Stock,
            StockChangeType.Decreased
        ));
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.InvalidProductStock)
                .WithData("Quantity", quantity);
        }

        var oldStock = Stock;
        Stock += quantity;

        AddLocalEvent(new ProductStockChangedEvent(
            Id,
            Name,
            oldStock,
            Stock,
            StockChangeType.Increased
        ));
    }
}
