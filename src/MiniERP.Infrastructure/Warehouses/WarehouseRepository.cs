
using Microsoft.EntityFrameworkCore;
using MiniERP.Application.Warehouses;
using MiniERP.Domain.Entities;
using MiniERP.Infrastructure.Persistence;

namespace MiniERP.Infrastructure.Warehouses;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly MiniERPDbContext _context;

    public WarehouseRepository(MiniERPDbContext context)
    {
        _context = context;
    }

    public Task<List<Warehouse>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        _context.Warehouses
            .AsNoTracking()
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);

    public Task<Warehouse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _context.Warehouses.FirstOrDefaultAsync(
            w => w.Id == id, cancellationToken);

    public Task<bool> CodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default) =>
        _context.Warehouses.AnyAsync(
            w => w.Code == code, cancellationToken);

    public Task<bool> CodeExistsAsync(
        string code,
        int excludeWarehouseId,
        CancellationToken cancellationToken = default) =>
        _context.Warehouses.AnyAsync(
            w => w.Code == code && w.Id != excludeWarehouseId,
            cancellationToken);

    public async Task AddAsync(
        Warehouse warehouse,
        CancellationToken cancellationToken = default) =>
        await _context.Warehouses.AddAsync(warehouse, cancellationToken);

    public Task UpdateAsync(
        Warehouse warehouse,
        CancellationToken cancellationToken = default)
    {
        _context.Warehouses.Update(warehouse);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
