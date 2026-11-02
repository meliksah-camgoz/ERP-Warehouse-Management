namespace MiniERP.Application.Assets.Dtos;

public class AssetDto
{
    public int Id { get; set; }

    public string AssetNumber { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }

    public string? AssignedTo { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }

    public string Status { get; set; } = string.Empty;
    public DateTime? WarrantyEndDate { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}