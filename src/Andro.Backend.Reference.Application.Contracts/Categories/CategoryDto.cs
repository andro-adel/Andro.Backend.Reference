using System;
using Volo.Abp.Application.Dtos;

namespace Andro.Backend.Reference.Categories;

public class CategoryDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
