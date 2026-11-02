using System.ComponentModel.DataAnnotations;

namespace MiniERP.Application.Assets.Dtos;

public class CreateAssetDto
{
    [Required]
    [MaxLength(50)]
    public string AssetNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string AssetType { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Model { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    [MaxLength(150)]
    public string? AssignedTo { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Kullanımda";

    public DateTime? WarrantyEndDate { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}