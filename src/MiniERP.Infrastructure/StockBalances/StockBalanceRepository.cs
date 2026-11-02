
using Microsoft.EntityFrameworkCore;
using MiniERP.Domain.Entities;
using MiniERP.Infrastructure.Persistence;
using MiniERP.Application.StockBalances;

namespace MiniERP.Infrastructure.StockBalances;

public class StockBalanceRepository : IStockBalanceRepository
{
    private readonly MiniERPDbContext _context;

    public StockBalanceRepository(MiniERPDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockBalance>> GetAllAsync()
    {
        return await _context.StockBalances
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .AsNoTracking()
            .OrderBy(x => x.Warehouse.Name)
            .ThenBy(x => x.Product.Name)
            .ToListAsync();
    }

    public async Task<StockBalance?> GetByProductAndWarehouseAsync(
        int productId,
        int warehouseId)
    {
        return await _context.StockBalances.FirstOrDefaultAsync(
            x => x.ProductId == productId &&
                 x.WarehouseId == warehouseId);
    }

    public Task<bool> ProductExistsAsync(int productId)
    {
        return _context.Products.AnyAsync(x => x.Id == productId);
    }

    public Task<bool> ProductIsActiveAsync(int productId)
    {
        return _context.Products.AnyAsync(x => x.Id == productId && x.IsActive);
    }

    public Task<bool> WarehouseExistsAsync(int warehouseId)
    {
        return _context.Warehouses.AnyAsync(x => x.Id == warehouseId);
    }

    public Task<bool> WarehouseIsActiveAsync(int warehouseId) =>
    _context.Warehouses.AnyAsync(x =>
        x.Id == warehouseId && x.IsActive);

    public async Task AddAsync(StockBalance stockBalance)
    {
        await _context.StockBalances.AddAsync(stockBalance);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task AddMovementAsync(StockMovement movement)
    {
        await _context.StockMovements.AddAsync(movement);
    }

    public async Task<List<StockMovement>> GetMovementsAsync()
    {
        return await _context.StockMovements
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();
    }
}
