using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Categories;
using MiniERP.Application.Categories.Dtos;

namespace MiniERP.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var category = await _service.GetByIdAsync(
            id, cancellationToken);

        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        CreateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var category = await _service.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(
            id, dto, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        var deactivated = await _service.DeactivateAsync(
            id, cancellationToken);

        return deactivated ? NoContent() : NotFound();
    }
}