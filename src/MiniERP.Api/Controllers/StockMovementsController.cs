using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.StockBalances;
using MiniERP.Application.StockBalances.Dtos;
namespace MiniERP.Api.Controllers;

[ApiController]
[Route("api/stock-movements")]
public class StockMovementsController : ControllerBase
{
    private readonly IStockBalanceService _service;

    public StockMovementsController(IStockBalanceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<StockMovementDto>>> GetAll()
    {
        var movements = await _service.GetMovementsAsync();
        return Ok(movements);
    }
}