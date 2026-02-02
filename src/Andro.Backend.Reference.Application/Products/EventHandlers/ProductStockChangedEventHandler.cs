using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Andro.Backend.Reference.Products.EventHandlers;

public class ProductStockChangedEventHandler 
    : ILocalEventHandler<ProductStockChangedEvent>,
      ITransientDependency
{
    private readonly ILogger<ProductStockChangedEventHandler> _logger;

    public ProductStockChangedEventHandler(
        ILogger<ProductStockChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(ProductStockChangedEvent eventData)
    {
        var changeIcon = eventData.ChangeType == StockChangeType.Increased 
            ? "📈" : "📉";

        _logger.LogInformation(
            "{Icon} Stock Changed: {ProductName} - {OldStock} → {NewStock} ({ChangeType}: {ChangeAmount})",
            changeIcon,
            eventData.ProductName,
            eventData.OldStock,
            eventData.NewStock,
            eventData.ChangeType,
            eventData.ChangeAmount
        );

        if (eventData.ChangeType == StockChangeType.Decreased 
            && eventData.NewStock < 10)
        {
            _logger.LogWarning(
                "⚠️ Low Stock Alert: {ProductName} - Only {Stock} items left!",
                eventData.ProductName,
                eventData.NewStock
            );
        }

        await Task.CompletedTask;
    }
}
