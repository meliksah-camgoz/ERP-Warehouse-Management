using MiniERP.Domain.Entities;

namespace MiniERP.Application.Products;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<List<Product>> GetActiveAsync(
    CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> SkuExistsAsync(
        string sku,
        int? excludeProductId = null,
        CancellationToken cancellationToken = default);

    Task<bool> CategoryExistsAsync(
        int categoryId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task<bool> HasStockBalancesAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<bool> HasStockMovementsAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}