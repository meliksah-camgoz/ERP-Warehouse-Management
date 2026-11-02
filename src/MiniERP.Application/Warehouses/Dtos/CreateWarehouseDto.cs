using System.ComponentModel.DataAnnotations;

namespace MiniERP.Application.Warehouses.Dtos;

public class CreateWarehouseDto
{
    [Required]
    [StringLength(30, MinimumLength = 2)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Address { get; set; }
}