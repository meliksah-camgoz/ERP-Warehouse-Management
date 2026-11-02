using System.ComponentModel.DataAnnotations;

namespace MiniERP.Application.Categories.Dtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}