
namespace MiniERP.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string SKU { get; set; } = string.Empty;

    public string? Barcode { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Unit { get; set; } = "Adet";

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

    public decimal VatRate { get; set; }

    public decimal MinStockLevel { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Category? Category { get; set; }
}
