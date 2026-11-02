
using MiniERP.Application.Warehouses.Dtos;
using MiniERP.Domain.Entities;

namespace MiniERP.Application.Warehouses;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _repository;

    public WarehouseService(IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WarehouseDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var warehouses = await _repository.GetAllAsync(cancellationToken);
        return warehouses.Select(MapToDto).ToList();
    }

    public async Task<WarehouseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _repository.GetByIdAsync(id, cancellationToken);
        return warehouse is null ? null : MapToDto(warehouse);
    }

    public async Task<WarehouseDto> CreateAsync(
        CreateWarehouseDto dto,
        CancellationToken cancellationToken = default)
    {
        var code = dto.Code.Trim().ToUpperInvariant();

        if (await _repository.CodeExistsAsync(code, cancellationToken))
            throw new ArgumentException("Bu depo kodu zaten kullanılıyor.");

        var warehouse = new Warehouse
        {
            Code = code,
            Name = dto.Name.Trim(),
            Address = NormalizeAddress(dto.Address),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(warehouse, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(warehouse);
    }

    public async Task<WarehouseDto?> UpdateAsync(
        int id,
        UpdateWarehouseDto dto,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _repository.GetByIdAsync(id, cancellationToken);

        if (warehouse is null)
            return null;

        var code = dto.Code.Trim().ToUpperInvariant();

        if (await _repository.CodeExistsAsync(
            code, id, cancellationToken))
        {
            throw new ArgumentException("Bu depo kodu zaten kullanılıyor.");
        }

        warehouse.Code = code;
        warehouse.Name = dto.Name.Trim();
        warehouse.Address = NormalizeAddress(dto.Address);

        await _repository.UpdateAsync(warehouse, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(warehouse);
    }

    public async Task<WarehouseDto?> SetActiveStatusAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _repository.GetByIdAsync(id, cancellationToken);

        if (warehouse is null)
            return null;

        warehouse.IsActive = isActive;

        await _repository.UpdateAsync(warehouse, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(warehouse);
    }

    private static string? NormalizeAddress(string? address) =>
        string.IsNullOrWhiteSpace(address) ? null : address.Trim();

    private static WarehouseDto MapToDto(Warehouse warehouse) =>
        new()
        {
            Id = warehouse.Id,
            Code = warehouse.Code,
            Name = warehouse.Name,
            Address = warehouse.Address,
            IsActive = warehouse.IsActive,
            CreatedAtUtc = warehouse.CreatedAtUtc
        };
}
