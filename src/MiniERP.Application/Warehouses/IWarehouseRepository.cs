
using MiniERP.Domain.Entities;

namespace MiniERP.Application.Warehouses;

public interface IWarehouseRepository
{
    Task<List<Warehouse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Warehouse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(
        string code,
        int excludeWarehouseId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Warehouse warehouse,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Warehouse warehouse,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
