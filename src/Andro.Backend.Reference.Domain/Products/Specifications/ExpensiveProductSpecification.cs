using System;
using System.Linq.Expressions;

namespace Andro.Backend.Reference.Products.Specifications;

public class ExpensiveProductSpecification : ProductSpecification
{
    private readonly decimal _minPrice;

    public ExpensiveProductSpecification(decimal minPrice = 1000)
    {
        _minPrice = minPrice;
    }

    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.Price >= _minPrice;
    }
}
