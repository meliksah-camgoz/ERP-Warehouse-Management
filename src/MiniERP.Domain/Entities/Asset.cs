namespace MiniERP.Domain.Entities;

public class Asset
{
    public int Id { get; set; }

    // Demirbaş bilgileri
    public string AssetNumber { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }

    // Zimmet ve konum bilgileri
    public string? AssignedTo { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }

    // Durum ve garanti
    public string Status { get; set; } = "Kullanımda";
    public DateTime? WarrantyEndDate { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}