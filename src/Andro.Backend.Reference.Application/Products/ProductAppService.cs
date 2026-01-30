using System;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<ProductDto> GetAsync(Guid id)
    {
        var product = await _repository.GetAsync(id);
        return MapToDto(product);
    }

    public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var products = await _repository.GetPagedListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting ?? "Name"
        );

        return new PagedResultDto<ProductDto>(
            totalCount,
            products.Select(MapToDto).ToList()
        );
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        var product = new Product(
            GuidGenerator.Create(),
            input.Name,
            input.Price,
            input.Stock,
            input.Description
        );

        await _repository.InsertAsync(product);
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input)
    {
        var product = await _repository.GetAsync(id);

        product.Name = input.Name;
        product.Price = input.Price;
        product.Stock = input.Stock;
        product.Description = input.Description;

        await _repository.UpdateAsync(product);
        return MapToDto(product);
    }

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
            CreationTime = product.CreationTime,
            CreatorId = product.CreatorId,
            LastModificationTime = product.LastModificationTime,
            LastModifierId = product.LastModifierId
        };
    }
}
