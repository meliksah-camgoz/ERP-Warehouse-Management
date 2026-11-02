
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.StockBalances;
using MiniERP.Application.StockBalances.Dtos;

namespace MiniERP.Api.Controllers;

[ApiController]
[Route("api/stock-balances")]
public class StockBalancesController : ControllerBase
{
    private readonly IStockBalanceService _service;

    public StockBalancesController(IStockBalanceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<StockBalanceDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost("entry")]
    public async Task<ActionResult<StockBalanceDto>> AddStock(
        StockEntryDto dto)
    {
        try
        {
            var result = await _service.AddStockAsync(dto);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPost("exit")]
    public async Task<ActionResult<StockBalanceDto>> RemoveStock(
        StockExitDto dto)
    {
        try
        {
            var result = await _service.RemoveStockAsync(dto);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}
