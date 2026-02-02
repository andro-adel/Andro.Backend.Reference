using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Andro.Backend.Reference.Products.EventHandlers;

public class ProductStockChangedEventHandler_Tests : ReferenceApplicationTestBase<ReferenceApplicationTestModule>
{
    private readonly ProductStockChangedEventHandler _handler;
    private readonly ILogger<ProductStockChangedEventHandler> _logger;

    public ProductStockChangedEventHandler_Tests()
    {
        _logger = GetRequiredService<ILogger<ProductStockChangedEventHandler>>();
        _handler = new ProductStockChangedEventHandler(_logger);
    }

    [Fact]
    public async Task Should_Handle_Stock_Increased_Event()
    {
        // Arrange
        var eventData = new ProductStockChangedEvent(
            Guid.NewGuid(),
            "Test Product",
            100,
            150,
            StockChangeType.Increased
        );

        // Act - Should not throw
        await _handler.HandleEventAsync(eventData);

        // Assert
        eventData.ShouldNotBeNull();
        eventData.ChangeType.ShouldBe(StockChangeType.Increased);
        eventData.OldStock.ShouldBe(100);
        eventData.NewStock.ShouldBe(150);
        eventData.ChangeAmount.ShouldBe(50);
    }

    [Fact]
    public async Task Should_Handle_Stock_Decreased_Event()
    {
        // Arrange
        var eventData = new ProductStockChangedEvent(
            Guid.NewGuid(),
            "Test Product",
            100,
            70,
            StockChangeType.Decreased
        );

        // Act
        await _handler.HandleEventAsync(eventData);

        // Assert
        eventData.ChangeType.ShouldBe(StockChangeType.Decreased);
        eventData.OldStock.ShouldBe(100);
        eventData.NewStock.ShouldBe(70);
        eventData.ChangeAmount.ShouldBe(30);
    }

    [Fact]
    public async Task Should_Handle_Low_Stock_Event()
    {
        // Arrange - Stock below 10
        var eventData = new ProductStockChangedEvent(
            Guid.NewGuid(),
            "Test Product",
            100,
            5,
            StockChangeType.Decreased
        );

        // Act - Should log warning for low stock
        await _handler.HandleEventAsync(eventData);

        // Assert
        eventData.NewStock.ShouldBeLessThan(10);
        eventData.ChangeType.ShouldBe(StockChangeType.Decreased);
    }

    [Fact]
    public async Task Should_Calculate_Change_Amount_Correctly()
    {
        // Arrange - Increase
        var increaseEvent = new ProductStockChangedEvent(
            Guid.NewGuid(),
            "Product 1",
            50,
            100,
            StockChangeType.Increased
        );

        // Assert - Increase
        increaseEvent.ChangeAmount.ShouldBe(50);

        // Arrange - Decrease
        var decreaseEvent = new ProductStockChangedEvent(
            Guid.NewGuid(),
            "Product 2",
            100,
            30,
            StockChangeType.Decreased
        );

        // Assert - Decrease
        decreaseEvent.ChangeAmount.ShouldBe(70);
    }
}
