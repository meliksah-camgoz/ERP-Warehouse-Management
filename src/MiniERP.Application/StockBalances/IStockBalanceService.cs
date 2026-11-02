
using MiniERP.Application.StockBalances.Dtos;

namespace MiniERP.Application.StockBalances;

public interface IStockBalanceService
{
    Task<List<StockBalanceDto>> GetAllAsync();

    Task<StockBalanceDto> AddStockAsync(StockEntryDto dto);

    Task<StockBalanceDto> RemoveStockAsync(StockExitDto dto);

    Task<List<StockMovementDto>> GetMovementsAsync();
}
