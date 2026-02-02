using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    Task<List<ProductDto>> GetLowStockProductsAsync(int threshold = 10);
    Task<List<ProductDto>> GetExpensiveProductsAsync(decimal minPrice = 1000);
    Task<List<ProductDto>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<List<ProductDto>> GetProductsByCategoryAsync(Guid categoryId);
}
