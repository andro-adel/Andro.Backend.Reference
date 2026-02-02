using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Andro.Backend.Reference.Products.EventHandlers;

public class ProductCreatedEventHandler 
    : ILocalEventHandler<ProductCreatedEvent>,
      ITransientDependency
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(ProductCreatedEvent eventData)
    {
        _logger.LogInformation(
            "🎉 New Product Created: {ProductName} (ID: {ProductId}) - Price: ${Price}, Stock: {Stock}",
            eventData.ProductName,
            eventData.ProductId,
            eventData.Price,
            eventData.Stock
        );

        await Task.CompletedTask;
    }
}
