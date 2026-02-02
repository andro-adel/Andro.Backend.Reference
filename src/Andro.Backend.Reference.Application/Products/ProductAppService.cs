using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Andro.Backend.Reference.Categories;
using Andro.Backend.Reference.Permissions;
using Andro.Backend.Reference.Products.Specifications;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;

namespace Andro.Backend.Reference.Products;

public class ProductAppService : ApplicationService, IProductAppService
{
    private readonly IRepository<Product, Guid> _repository;
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly ILocalEventBus _localEventBus;

    public ProductAppService(
        IRepository<Product, Guid> repository,
        IRepository<Category, Guid> categoryRepository,
        ILocalEventBus localEventBus)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _localEventBus = localEventBus;
    }

    [Authorize(ReferencePermissions.Products.Default)]
    public async Task<ProductDto> GetAsync(Guid id)
    {
        var product = await _repository.FindAsync(id, includeDetails: true);

        if (product == null)
        {
            throw new EntityNotFoundException(typeof(Product), id);
        }

        return MapToDto(product);
    }

    [Authorize(ReferencePermissions.Products.Default)]
    public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var products = await _repository.GetListAsync(includeDetails: true);

        return new PagedResultDto<ProductDto>(
            totalCount,
            products.Select(MapToDto).ToList()
        );
    }

    [Authorize(ReferencePermissions.Products.Create)]
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        // Check if category exists
        var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == input.CategoryId);
        if (!categoryExists)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.CategoryNotFound)
                .WithData("CategoryId", input.CategoryId);
        }

        // Check for duplicate product name
        var existingProduct = await _repository
            .FirstOrDefaultAsync(p => p.Name == input.Name);

        if (existingProduct != null)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.DuplicateProductName)
                .WithData("ProductName", input.Name);
        }

        var product = new Product(
            GuidGenerator.Create(),
            input.Name,
            input.Price,
            input.Stock,
            input.CategoryId,
            input.Description
        );

        await _repository.InsertAsync(product);

        // Publish ProductCreatedEvent
        await _localEventBus.PublishAsync(
            new ProductCreatedEvent(
                product.Id,
                product.Name,
                product.Price,
                product.Stock,
                product.CategoryId
            )
        );

        return MapToDto(product);
    }

    [Authorize(ReferencePermissions.Products.Edit)]
    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input)
    {
        var product = await _repository.GetAsync(id, includeDetails: true);

        // Check if category exists
        var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == input.CategoryId);
        if (!categoryExists)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.CategoryNotFound)
                .WithData("CategoryId", input.CategoryId);
        }

        // Check for duplicate name (excluding current product)
        var existingProduct = await _repository
            .FirstOrDefaultAsync(p => p.Name == input.Name && p.Id != id);

        if (existingProduct != null)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.DuplicateProductName)
                .WithData("ProductName", input.Name);
        }

        // Use setter methods (Domain validation happens here)
        product.SetName(input.Name);
        product.SetPrice(input.Price);
        product.SetStock(input.Stock);
        product.CategoryId = input.CategoryId;
        product.Description = input.Description;

        await _repository.UpdateAsync(product);
        return MapToDto(product);
    }

    [Authorize(ReferencePermissions.Products.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            CreationTime = product.CreationTime,
            CreatorId = product.CreatorId,
            LastModificationTime = product.LastModificationTime,
            LastModifierId = product.LastModifierId
        };
    }

    [Authorize(ReferencePermissions.Products.Default)]
    public async Task<List<ProductDto>> GetLowStockProductsAsync(int threshold = 10)
    {
        var specification = new LowStockProductSpecification(threshold);

        var queryable = await _repository.GetQueryableAsync();
        var products = queryable
            .Where(specification.ToExpression())
            .ToList();

        return products.Select(MapToDto).ToList();
    }

    [Authorize(ReferencePermissions.Products.Default)]
    public async Task<List<ProductDto>> GetExpensiveProductsAsync(decimal minPrice = 1000)
    {
        var specification = new ExpensiveProductSpecification(minPrice);

        var queryable = await _repository.GetQueryableAsync();
        var products = queryable
            .Where(specification.ToExpression())
            .ToList();

        return products.Select(MapToDto).ToList();
    }

    [Authorize(ReferencePermissions.Products.Default)]
    public async Task<List<ProductDto>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var specification = new ProductInPriceRangeSpecification(minPrice, maxPrice);

        var queryable = await _repository.GetQueryableAsync();
        var products = queryable
            .Where(specification.ToExpression())
            .ToList();

        return products.Select(MapToDto).ToList();
    }

    [Authorize(ReferencePermissions.Products.Default)]
    public async Task<List<ProductDto>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var specification = new ProductByCategorySpecification(categoryId);

        var queryable = await _repository.GetQueryableAsync();
        var products = queryable
            .Where(specification.ToExpression())
            .ToList();

        return products.Select(MapToDto).ToList();
    }
}
