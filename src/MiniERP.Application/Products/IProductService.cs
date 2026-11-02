
using MiniERP.Application.Products.Dtos;

namespace MiniERP.Application.Products;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductDto> CreateAsync(
        CreateProductDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}
