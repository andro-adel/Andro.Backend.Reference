using System;
using Andro.Backend.Reference.Categories;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Andro.Backend.Reference.Products;

public class Product_Tests : ReferenceDomainTestBase<ReferenceDomainTestModule>
{
    [Fact]
    public void Should_Create_Product_With_Valid_Data()
    {
        // Arrange & Act
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid(),
            "Test description"
        );

        // Assert
        product.ShouldNotBeNull();
        product.Name.ShouldBe("Test Product");
        product.Price.ShouldBe(99.99m);
        product.Stock.ShouldBe(10);
        product.Description.ShouldBe("Test description");
    }

    [Fact]
    public void Should_Set_Valid_Name()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Original Name",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act
        product.SetName("New Name");

        // Assert
        product.Name.ShouldBe("New Name");
    }

    [Fact]
    public void Should_Not_Allow_Empty_Name()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Original Name",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
        {
            product.SetName("");
        });
    }

    [Fact]
    public void Should_Set_Valid_Price()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act
        product.SetPrice(199.99m);

        // Assert
        product.Price.ShouldBe(199.99m);
    }

    [Fact]
    public void Should_Not_Allow_Price_Below_Minimum()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<BusinessException>(() =>
        {
            product.SetPrice(0m); // Below minimum
        });
    }

    [Fact]
    public void Should_Not_Allow_Price_Above_Maximum()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<BusinessException>(() =>
        {
            product.SetPrice(2000000m); // Above maximum
        });
    }

    [Fact]
    public void Should_Set_Valid_Stock()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act
        product.SetStock(50);

        // Assert
        product.Stock.ShouldBe(50);
    }

    [Fact]
    public void Should_Not_Allow_Negative_Stock()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<BusinessException>(() =>
        {
            product.SetStock(-5);
        });
    }

    [Fact]
    public void Should_Not_Allow_Stock_Above_Maximum()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<BusinessException>(() =>
        {
            product.SetStock(200000); // Above maximum
        });
    }

    [Fact]
    public void Should_Increase_Stock()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            100,
            Guid.NewGuid()
        );

        // Act
        product.IncreaseStock(50);

        // Assert
        product.Stock.ShouldBe(150);
    }

    [Fact]
    public void Should_Not_Allow_Increase_With_Zero_Or_Negative()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            100,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<BusinessException>(() =>
        {
            product.IncreaseStock(0);
        });

        Should.Throw<BusinessException>(() =>
        {
            product.IncreaseStock(-10);
        });
    }

    [Fact]
    public void Should_Decrease_Stock()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            100,
            Guid.NewGuid()
        );

        // Act
        product.DecreaseStock(30);

        // Assert
        product.Stock.ShouldBe(70);
    }

    [Fact]
    public void Should_Not_Allow_Decrease_With_Zero_Or_Negative()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            100,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<BusinessException>(() =>
        {
            product.DecreaseStock(0);
        });

        Should.Throw<BusinessException>(() =>
        {
            product.DecreaseStock(-10);
        });
    }

    [Fact]
    public void Should_Throw_InsufficientStockException_When_Stock_Not_Enough()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            99.99m,
            10,
            Guid.NewGuid()
        );

        // Act & Assert
        Should.Throw<InsufficientStockException>(() =>
        {
            product.DecreaseStock(20); // More than available
        });
    }
}
