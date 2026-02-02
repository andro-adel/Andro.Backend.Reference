using System;
using System.Linq;
using System.Threading.Tasks;
using Andro.Backend.Reference.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Andro.Backend.Reference.Products;

public class ProductAppService : ApplicationService, IProductAppService
{
    private readonly IRepository<Product, Guid> _repository;

    public ProductAppService(IRepository<Product, Guid> repository)
    {
        _repository = repository;
    }

    [Authorize(ReferencePermissions.Products.Default)]
    public async Task<ProductDto> GetAsync(Guid id)
    {
        var product = await _repository.GetAsync(id, includeDetails: true);
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
        var product = new Product(
            GuidGenerator.Create(),
            input.Name,
            input.Price,
            input.Stock,
            input.CategoryId,
            input.Description
        );

        await _repository.InsertAsync(product);
        return MapToDto(product);
    }

    [Authorize(ReferencePermissions.Products.Edit)]
    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input)
    {
        var product = await _repository.GetAsync(id, includeDetails: true);

        product.Name = input.Name;
        product.Price = input.Price;
        product.Stock = input.Stock;
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

    private static ProductDto MapToDto(Product product)
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
}
