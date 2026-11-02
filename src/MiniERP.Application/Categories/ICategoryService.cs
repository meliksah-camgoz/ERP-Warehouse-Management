using MiniERP.Application.Categories.Dtos;

namespace MiniERP.Application.Categories;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<CategoryDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<CategoryDto> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        int id,
        CancellationToken cancellationToken = default);
}