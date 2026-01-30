using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Andro.Backend.Reference.Products;

public interface IProductAppService : 
    ICrudAppService<
        ProductDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateProductDto,
        UpdateProductDto>
{
}
