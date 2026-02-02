using System;
using System.Linq.Expressions;

namespace Andro.Backend.Reference.Products.Specifications;

public class LowStockProductSpecification : ProductSpecification
{
    private readonly int _threshold;

    public LowStockProductSpecification(int threshold = 10)
    {
        _threshold = threshold;
    }

    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.Stock < _threshold;
    }
}
