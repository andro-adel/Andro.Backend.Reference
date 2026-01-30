using System;
using Volo.Abp.Application.Dtos;

namespace Andro.Backend.Reference.Products;

public class ProductDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
    
    public string Description { get; set; }
}
