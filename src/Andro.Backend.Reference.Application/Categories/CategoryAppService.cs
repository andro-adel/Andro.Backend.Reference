using System;
using System.Linq;
using System.Threading.Tasks;
using Andro.Backend.Reference.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Andro.Backend.Reference.Categories;

public class CategoryAppService : ApplicationService, ICategoryAppService
{
    private readonly IRepository<Category, Guid> _repository;

    public CategoryAppService(IRepository<Category, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<CategoryDto> GetAsync(Guid id)
    {
        var category = await _repository.GetAsync(id);
        return MapToDto(category);
    }

    public async Task<PagedResultDto<CategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var categories = await _repository.GetPagedListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting ?? "Name"
        );

        return new PagedResultDto<CategoryDto>(
            totalCount,
            categories.Select(MapToDto).ToList()
        );
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto input)
    {
        var category = new Category(
            GuidGenerator.Create(),
            input.Name,
            input.Description
        );

        await _repository.InsertAsync(category);
        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto input)
    {
        var category = await _repository.GetAsync(id);

        category.Name = input.Name;
        category.Description = input.Description;

        await _repository.UpdateAsync(category);
        return MapToDto(category);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
