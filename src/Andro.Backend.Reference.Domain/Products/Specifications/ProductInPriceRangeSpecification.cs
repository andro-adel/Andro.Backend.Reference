using System;
using System.Linq.Expressions;

namespace Andro.Backend.Reference.Products.Specifications;

public class ProductInPriceRangeSpecification : ProductSpecification
{
    private readonly decimal _minPrice;
    private readonly decimal _maxPrice;

    public ProductInPriceRangeSpecification(decimal minPrice, decimal maxPrice)
    {
        _minPrice = minPrice;
        _maxPrice = maxPrice;
    }

    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.Price >= _minPrice && p.Price <= _maxPrice;
    }
}
