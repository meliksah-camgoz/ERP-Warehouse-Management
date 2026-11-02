
using System.ComponentModel.DataAnnotations;

namespace MiniERP.Application.StockBalances.Dtos;

public class StockExitDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int WarehouseId { get; set; }

    public decimal Quantity { get; set; }
}
