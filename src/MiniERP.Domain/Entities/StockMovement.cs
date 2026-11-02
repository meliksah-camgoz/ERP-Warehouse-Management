
namespace MiniERP.Domain.Entities;

public class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    // StockEntry veya StockExit
    public string MovementType { get; set; } = string.Empty;

    // Giriş ve çıkış miktarı pozitif tutulur.
    public decimal Quantity { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public string? Description { get; set; }
}
