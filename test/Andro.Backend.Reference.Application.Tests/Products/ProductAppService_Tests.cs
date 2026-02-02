using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace Andro.Backend.Reference.Products;

public class ProductAppService_Tests : ReferenceApplicationTestBase<ReferenceApplicationTestModule>
{
    private readonly IProductAppService _productAppService;
    private readonly Guid _testCategoryId = Guid.Parse("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f");

    public ProductAppService_Tests()
    {
        _productAppService = GetRequiredService<IProductAppService>();
    }

    [Fact]
    public async Task Should_Get_Product_List()
    {
        // Act
        var result = await _productAppService.GetListAsync(
            new PagedAndSortedResultRequestDto()
        );

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldNotBeNull();
        result.Items.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Get_Product_By_Id()
    {
        // Arrange - Create a product first
        var createInput = new CreateProductDto
        {
            Name = "Test Product For Get",
            Price = 99.99m,
            Stock = 10,
            CategoryId = _testCategoryId,
            Description = "Test description"
        };
        var created = await _productAppService.CreateAsync(createInput);

        // Act
        var result = await _productAppService.GetAsync(created.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe(createInput.Name);
        result.Price.ShouldBe(createInput.Price);
    }

    [Fact]
    public async Task Should_Throw_EntityNotFoundException_For_NonExistent_Product()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await _productAppService.GetAsync(nonExistentId);
        });
    }

    [Fact]
    public async Task Should_Create_Valid_Product()
    {
        // Arrange
        var input = new CreateProductDto
        {
            Name = "New Test Product " + Guid.NewGuid().ToString().Substring(0, 8),
            Price = 149.99m,
            Stock = 25,
            CategoryId = _testCategoryId,
            Description = "Test product description"
        };

        // Act
        var result = await _productAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe(input.Name);
        result.Price.ShouldBe(input.Price);
        result.Stock.ShouldBe(input.Stock);
        result.Description.ShouldBe(input.Description);
    }

    [Fact]
    public async Task Should_Not_Create_Product_With_Invalid_Category()
    {
        // Arrange
        var input = new CreateProductDto
        {
            Name = "Test Product",
            Price = 99.99m,
            Stock = 10,
            CategoryId = Guid.NewGuid(), // Non-existent category
            Description = "Test"
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.CreateAsync(input);
        });

        exception.Code.ShouldBe(ReferenceDomainErrorCodes.CategoryNotFound);
    }

    [Fact]
    public async Task Should_Not_Create_Product_With_Duplicate_Name()
    {
        // Arrange - Create first product
        var uniqueName = "Unique Product " + Guid.NewGuid().ToString().Substring(0, 8);
        var input1 = new CreateProductDto
        {
            Name = uniqueName,
            Price = 99.99m,
            Stock = 10,
            CategoryId = _testCategoryId
        };
        await _productAppService.CreateAsync(input1);

        // Act & Assert - Try to create duplicate
        var input2 = new CreateProductDto
        {
            Name = uniqueName, // Same name
            Price = 199.99m,
            Stock = 20,
            CategoryId = _testCategoryId
        };

        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.CreateAsync(input2);
        });

        exception.Code.ShouldBe(ReferenceDomainErrorCodes.DuplicateProductName);
    }

    [Fact]
    public async Task Should_Update_Product()
    {
        // Arrange - Create product first
        var createInput = new CreateProductDto
        {
            Name = "Original Product " + Guid.NewGuid().ToString().Substring(0, 8),
            Price = 99.99m,
            Stock = 10,
            CategoryId = _testCategoryId,
            Description = "Original description"
        };
        var created = await _productAppService.CreateAsync(createInput);

        // Act - Update
        var updateInput = new UpdateProductDto
        {
            Name = "Updated Product " + Guid.NewGuid().ToString().Substring(0, 8),
            Price = 199.99m,
            Stock = 20,
            CategoryId = _testCategoryId,
            Description = "Updated description"
        };
        var updated = await _productAppService.UpdateAsync(created.Id, updateInput);

        // Assert
        updated.Id.ShouldBe(created.Id);
        updated.Name.ShouldBe(updateInput.Name);
        updated.Price.ShouldBe(updateInput.Price);
        updated.Stock.ShouldBe(updateInput.Stock);
        updated.Description.ShouldBe(updateInput.Description);
    }

    [Fact]
    public async Task Should_Not_Update_Product_With_Invalid_Category()
    {
        // Arrange - Create product
        var createInput = new CreateProductDto
        {
            Name = "Test Product " + Guid.NewGuid().ToString().Substring(0, 8),
            Price = 99.99m,
            Stock = 10,
            CategoryId = _testCategoryId
        };
        var created = await _productAppService.CreateAsync(createInput);

        // Act & Assert - Update with invalid category
        var updateInput = new UpdateProductDto
        {
            Name = "Updated Name",
            Price = 199.99m,
            Stock = 20,
            CategoryId = Guid.NewGuid() // Non-existent
        };

        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.UpdateAsync(created.Id, updateInput);
        });

        exception.Code.ShouldBe(ReferenceDomainErrorCodes.CategoryNotFound);
    }

    [Fact]
    public async Task Should_Not_Update_Product_With_Duplicate_Name()
    {
        // Arrange - Create two products
        var product1 = await _productAppService.CreateAsync(new CreateProductDto
        {
            Name = "Product One " + Guid.NewGuid().ToString().Substring(0, 8),
            Price = 99.99m,
            Stock = 10,
            CategoryId = _testCategoryId
        });

        var product2Name = "Product Two " + Guid.NewGuid().ToString().Substring(0, 8);
        var product2 = await _productAppService.CreateAsync(new CreateProductDto
        {
            Name = product2Name,
            Price = 149.99m,
            Stock = 15,
            CategoryId = _testCategoryId
        });

        // Act & Assert - Try to update product1 with product2's name
        var updateInput = new UpdateProductDto
        {
            Name = product2Name, // Duplicate name
            Price = 99.99m,
            Stock = 10,
            CategoryId = _testCategoryId
        };

        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.UpdateAsync(product1.Id, updateInput);
        });

        exception.Code.ShouldBe(ReferenceDomainErrorCodes.DuplicateProductName);
    }

    [Fact]
    public async Task Should_Delete_Product()
    {
        // Arrange - Create product
        var createInput = new CreateProductDto
        {
            Name = "To Be Deleted " + Guid.NewGuid().ToString().Substring(0, 8),
            Price = 99.99m,
            Stock = 10,
            CategoryId = _testCategoryId
        };
        var created = await _productAppService.CreateAsync(createInput);

        // Act
        await _productAppService.DeleteAsync(created.Id);

        // Assert - Should not find deleted product
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await _productAppService.GetAsync(created.Id);
        });
    }

    [Fact]
    public async Task Should_Validate_Price_Range()
    {
        // Arrange & Act & Assert - Invalid low price
        var lowPriceInput = new CreateProductDto
        {
            Name = "Low Price Product",
            Price = 0m, // Below minimum
            Stock = 10,
            CategoryId = _testCategoryId
        };

        await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.CreateAsync(lowPriceInput);
        });

        // Arrange & Act & Assert - Invalid high price
        var highPriceInput = new CreateProductDto
        {
            Name = "High Price Product",
            Price = 2000000m, // Above maximum
            Stock = 10,
            CategoryId = _testCategoryId
        };

        await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.CreateAsync(highPriceInput);
        });
    }

    [Fact]
    public async Task Should_Validate_Stock_Range()
    {
        // Arrange & Act & Assert - Invalid negative stock
        var negativeStockInput = new CreateProductDto
        {
            Name = "Negative Stock Product",
            Price = 99.99m,
            Stock = -5, // Negative
            CategoryId = _testCategoryId
        };

        await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.CreateAsync(negativeStockInput);
        });

        // Arrange & Act & Assert - Invalid high stock
        var highStockInput = new CreateProductDto
        {
            Name = "High Stock Product",
            Price = 99.99m,
            Stock = 200000, // Above maximum
            CategoryId = _testCategoryId
        };

        await Should.ThrowAsync<BusinessException>(async () =>
        {
            await _productAppService.CreateAsync(highStockInput);
        });
    }
}
