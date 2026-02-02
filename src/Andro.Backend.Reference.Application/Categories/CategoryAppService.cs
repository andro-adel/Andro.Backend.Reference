using System;
using System.Linq;
using System.Threading.Tasks;
using Andro.Backend.Reference.Permissions;
using Andro.Backend.Reference.Products;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Andro.Backend.Reference.Categories;

public class CategoryAppService : ApplicationService, ICategoryAppService
{
    private readonly IRepository<Category, Guid> _repository;
    private readonly IRepository<Product, Guid> _productRepository;

    public CategoryAppService(
        IRepository<Category, Guid> repository,
        IRepository<Product, Guid> productRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
    }

    public async Task<CategoryDto> GetAsync(Guid id)
    {
        var category = await _repository.FindAsync(id);

        if (category == null)
        {
            throw new EntityNotFoundException(typeof(Category), id);
        }

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
        // Check for duplicate category name
        var existingCategory = await _repository
            .FirstOrDefaultAsync(c => c.Name == input.Name);

        if (existingCategory != null)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.DuplicateCategoryName)
                .WithData("CategoryName", input.Name);
        }

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

        // Check for duplicate name (excluding current category)
        var existingCategory = await _repository
            .FirstOrDefaultAsync(c => c.Name == input.Name && c.Id != id);

        if (existingCategory != null)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.DuplicateCategoryName)
                .WithData("CategoryName", input.Name);
        }

        category.Name = input.Name;
        category.Description = input.Description;

        await _repository.UpdateAsync(category);
        return MapToDto(category);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _repository.GetAsync(id);

        // Check if category has products
        var hasProducts = await _productRepository.AnyAsync(p => p.CategoryId == id);

        if (hasProducts)
        {
            throw new BusinessException(ReferenceDomainErrorCodes.CategoryHasProducts)
                .WithData("CategoryName", category.Name)
                .WithData("CategoryId", id);
        }

        await _repository.DeleteAsync(category);
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
