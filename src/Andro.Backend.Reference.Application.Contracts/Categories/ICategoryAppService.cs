using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Andro.Backend.Reference.Categories;

public interface ICategoryAppService : 
    ICrudAppService<CategoryDto, Guid, PagedAndSortedResultRequestDto, CreateCategoryDto, UpdateCategoryDto>
{
}
