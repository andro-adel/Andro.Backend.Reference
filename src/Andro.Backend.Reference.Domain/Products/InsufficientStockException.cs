using Volo.Abp;

namespace Andro.Backend.Reference.Products;

public class InsufficientStockException : BusinessException
{
    public InsufficientStockException(
        string productName,
        int requestedQuantity,
        int availableStock)
        : base(ReferenceDomainErrorCodes.InsufficientStock)
    {
        WithData("ProductName", productName);
        WithData("RequestedQuantity", requestedQuantity);
        WithData("AvailableStock", availableStock);
    }
}
