
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Warehouses;
using MiniERP.Application.Warehouses.Dtos;

namespace MiniERP.Api.Controllers;

[ApiController]
[Route("api/warehouses")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _service;

    public WarehousesController(IWarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<WarehouseDto>>> GetAll(
        CancellationToken cancellationToken) =>
        Ok(await _service.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WarehouseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var warehouse = await _service.GetByIdAsync(id, cancellationToken);
        return warehouse is null ? NotFound() : Ok(warehouse);
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseDto>> Create(
        CreateWarehouseDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await _service.CreateAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = warehouse.Id },
                warehouse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WarehouseDto>> Update(
        int id,
        UpdateWarehouseDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await _service.UpdateAsync(
                id, dto, cancellationToken);

            return warehouse is null ? NotFound() : Ok(warehouse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    public sealed class WarehouseStatusDto
    {
        public bool IsActive { get; set; }
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<WarehouseDto>> SetStatus(
        int id,
        WarehouseStatusDto dto,
        CancellationToken cancellationToken)
    {
        var warehouse = await _service.SetActiveStatusAsync(
            id, dto.IsActive, cancellationToken);

        return warehouse is null ? NotFound() : Ok(warehouse);
    }
}
