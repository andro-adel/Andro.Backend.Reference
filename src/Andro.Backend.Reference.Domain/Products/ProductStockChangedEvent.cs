using System;

namespace Andro.Backend.Reference.Products;

public class ProductStockChangedEvent
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int OldStock { get; set; }
    public int NewStock { get; set; }
    public int ChangeAmount { get; set; }
    public StockChangeType ChangeType { get; set; }
    public DateTime ChangedAt { get; set; }

    public ProductStockChangedEvent(
        Guid productId,
        string productName,
        int oldStock,
        int newStock,
        StockChangeType changeType)
    {
        ProductId = productId;
        ProductName = productName;
        OldStock = oldStock;
        NewStock = newStock;
        ChangeAmount = Math.Abs(newStock - oldStock);
        ChangeType = changeType;
        ChangedAt = DateTime.UtcNow;
    }
}

public enum StockChangeType
{
    Increased,
    Decreased
}
