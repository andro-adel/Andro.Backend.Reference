using System;
using System.Linq.Expressions;

namespace Andro.Backend.Reference.Products.Specifications;

public class ProductByCategorySpecification : ProductSpecification
{
    private readonly Guid _categoryId;

    public ProductByCategorySpecification(Guid categoryId)
    {
        _categoryId = categoryId;
    }

    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.CategoryId == _categoryId;
    }
}
