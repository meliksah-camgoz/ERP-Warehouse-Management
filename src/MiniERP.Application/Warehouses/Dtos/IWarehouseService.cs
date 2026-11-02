
using MiniERP.Application.Warehouses.Dtos;

namespace MiniERP.Application.Warehouses;

public interface IWarehouseService
{
    Task<List<WarehouseDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<WarehouseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<WarehouseDto> CreateAsync(
        CreateWarehouseDto dto,
        CancellationToken cancellationToken = default);

    Task<WarehouseDto?> UpdateAsync(
        int id,
        UpdateWarehouseDto dto,
        CancellationToken cancellationToken = default);

    Task<WarehouseDto?> SetActiveStatusAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default);
}
