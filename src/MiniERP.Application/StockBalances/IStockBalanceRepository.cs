
using MiniERP.Domain.Entities;

namespace MiniERP.Application.StockBalances;

public interface IStockBalanceRepository
{
    Task<List<StockBalance>> GetAllAsync();
    Task<StockBalance?> GetByProductAndWarehouseAsync(
        int productId,
        int warehouseId);
    Task<bool> ProductExistsAsync(int productId);
    Task<bool> WarehouseExistsAsync(int warehouseId);
    Task<bool> WarehouseIsActiveAsync(int warehouseId);
    Task<bool> ProductIsActiveAsync(int productId);
    Task AddAsync(StockBalance stockBalance);
    Task AddMovementAsync(StockMovement movement);
    Task<List<StockMovement>> GetMovementsAsync();

    Task SaveChangesAsync();
}
