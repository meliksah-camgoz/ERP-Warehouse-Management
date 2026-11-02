namespace MiniERP.Application.StockBalances.Dtos;

public class StockMovementDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    public string MovementType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public string? Description { get; set; }
}