using System;

namespace Andro.Backend.Reference.Products.Jobs;

public class LowStockAlertJobArgs
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }

    public LowStockAlertJobArgs()
    {
    }

    public LowStockAlertJobArgs(
        Guid productId,
        string productName,
        int currentStock,
        int minimumStock)
    {
        ProductId = productId;
        ProductName = productName;
        CurrentStock = currentStock;
        MinimumStock = minimumStock;
    }
}
