using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;

namespace Andro.Backend.Reference.Products.Workers;

public class StockCheckWorker : AsyncPeriodicBackgroundWorkerBase
{
    public StockCheckWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory) 
        : base(timer, serviceScopeFactory)
    {
        // Run every 5 minutes (300,000 milliseconds)
        Timer.Period = 5 * 60 * 1000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var logger = workerContext
            .ServiceProvider
            .GetRequiredService<ILogger<StockCheckWorker>>();

        var productRepository = workerContext
            .ServiceProvider
            .GetRequiredService<IRepository<Product, Guid>>();

        logger.LogInformation(
            "🔍 Stock check worker started at {Time}",
            DateTime.Now
        );

        // Get all products with low stock (less than 10)
        var lowStockProducts = (await productRepository.GetQueryableAsync())
            .Where(p => p.Stock < 10)
            .ToList();

        if (lowStockProducts.Count > 0)
        {
            logger.LogWarning(
                "⚠️ Found {Count} products with low stock",
                lowStockProducts.Count
            );

            foreach (var product in lowStockProducts)
            {
                logger.LogWarning(
                    "📦 Low Stock: {ProductName} (ID: {ProductId}) - Current: {Stock}",
                    product.Name,
                    product.Id,
                    product.Stock
                );
            }
        }
        else
        {
            logger.LogInformation("✅ All products have sufficient stock");
        }

        logger.LogInformation(
            "✅ Stock check worker completed at {Time}",
            DateTime.Now
        );
    }
}
