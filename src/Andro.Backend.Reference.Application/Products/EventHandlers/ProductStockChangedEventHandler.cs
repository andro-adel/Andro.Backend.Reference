using System.Threading.Tasks;
using Andro.Backend.Reference.Products.Jobs;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Andro.Backend.Reference.Products.EventHandlers;

public class ProductStockChangedEventHandler
    : ILocalEventHandler<ProductStockChangedEvent>,
      ITransientDependency
{
    private readonly ILogger<ProductStockChangedEventHandler> _logger;
    private readonly IBackgroundJobManager _backgroundJobManager;

    public ProductStockChangedEventHandler(
        ILogger<ProductStockChangedEventHandler> logger,
        IBackgroundJobManager backgroundJobManager)
    {
        _logger = logger;
        _backgroundJobManager = backgroundJobManager;
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

            // Enqueue background job for low stock alert
            await _backgroundJobManager.EnqueueAsync(
                new LowStockAlertJobArgs(
                    eventData.ProductId,
                    eventData.ProductName,
                    eventData.NewStock,
                    10 // minimum stock threshold
                )
            );

            _logger.LogInformation(
                "🔔 Low stock alert job enqueued for product: {ProductName}",
                eventData.ProductName
            );
        }
    }
}
