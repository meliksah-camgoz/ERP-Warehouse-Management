using System.ComponentModel.DataAnnotations;

namespace MiniERP.Application.Products.Dtos;

public class CreateProductDto
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string SKU { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Barcode { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = "Adet";

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal PurchasePrice { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal SalePrice { get; set; }

    [Range(typeof(decimal), "0", "100")]
    public decimal VatRate { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal MinStockLevel { get; set; }
}