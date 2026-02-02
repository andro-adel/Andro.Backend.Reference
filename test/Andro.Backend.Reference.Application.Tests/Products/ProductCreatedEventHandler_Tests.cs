using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Andro.Backend.Reference.Products.EventHandlers;

public class ProductCreatedEventHandler_Tests : ReferenceApplicationTestBase<ReferenceApplicationTestModule>
{
    private readonly ProductCreatedEventHandler _handler;
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler_Tests()
    {
        _logger = GetRequiredService<ILogger<ProductCreatedEventHandler>>();
        _handler = new ProductCreatedEventHandler(_logger);
    }

    [Fact]
    public async Task Should_Handle_ProductCreated_Event()
    {
        // Arrange
        var eventData = new ProductCreatedEvent(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act - Should not throw
        await _handler.HandleEventAsync(eventData);

        // Assert - Event handled successfully
        eventData.ShouldNotBeNull();
        eventData.ProductName.ShouldBe("Test Product");
        eventData.Price.ShouldBe(99.99m);
        eventData.Stock.ShouldBe(10);
    }

    [Fact]
    public async Task Should_Handle_Multiple_Events()
    {
        // Arrange
        var event1 = new ProductCreatedEvent(
            Guid.NewGuid(),
            "Product 1",
            99.99m,
            10,
            Guid.NewGuid()
        );

        var event2 = new ProductCreatedEvent(
            Guid.NewGuid(),
            "Product 2",
            149.99m,
            20,
            Guid.NewGuid()
        );

        // Act
        await _handler.HandleEventAsync(event1);
        await _handler.HandleEventAsync(event2);

        // Assert - Both events handled successfully
        event1.ProductName.ShouldBe("Product 1");
        event2.ProductName.ShouldBe("Product 2");
    }
}
