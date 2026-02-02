using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;

namespace Andro.Backend.Reference.Products.Specifications;

public abstract class ProductSpecification : Specification<Product>
{
    public override abstract Expression<Func<Product, bool>> ToExpression();
}
