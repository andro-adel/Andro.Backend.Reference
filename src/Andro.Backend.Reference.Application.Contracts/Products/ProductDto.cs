using System;
using Volo.Abp.Application.Dtos;

namespace Andro.Backend.Reference.Products;

public class ProductDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
