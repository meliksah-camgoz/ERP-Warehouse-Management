using MiniERP.Application.Categories.Dtos;
using MiniERP.Domain.Entities;

namespace MiniERP.Application.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _repository.GetAllAsync(cancellationToken);

        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(
            id, cancellationToken);

        return category is null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(category, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(
            id, cancellationToken);

        if (category is null || !category.IsActive)
            return false;

        category.Name = dto.Name.Trim();
        category.Description = dto.Description?.Trim();

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(
            id, cancellationToken);

        if (category is null || !category.IsActive)
            return false;

        category.IsActive = false;

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAtUtc = category.CreatedAtUtc
        };
    }
}