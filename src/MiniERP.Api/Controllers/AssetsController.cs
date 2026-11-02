using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Assets;
using MiniERP.Application.Assets.Dtos;
using Microsoft.AspNetCore.Http;


namespace MiniERP.Api.Controllers;

[ApiController]
[Route("api/assets")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _service;

    public AssetsController(IAssetService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<AssetDto>>> GetAll()
    {
        var assets = await _service.GetAllAsync();
        return Ok(assets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssetDto>> GetById(int id)
    {
        var asset = await _service.GetByIdAsync(id);

        if (asset is null)
            return NotFound(new { message = "Envanter kaydı bulunamadı." });

        return Ok(asset);
    }

    [HttpPost]
    public async Task<ActionResult<AssetDto>> Create(CreateAssetDto dto)
    {
        try
        {
            var asset = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = asset.Id },
                asset);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("import-excel")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImportAssetsResultDto>> ImportExcel(
    IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "Lütfen boş olmayan bir Excel dosyası seçin."
            });
        }

        if (!Path.GetExtension(file.FileName)
            .Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Yalnızca .xlsx uzantılı Excel dosyaları kabul edilir."
            });
        }

        using var stream = file.OpenReadStream();

        try
        {
            var result = await _service.ImportExcelAsync(stream);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(new
            {
                message = "Excel dosyası okunamadı. Dosya biçimini kontrol edin."
            });
        }
    }
}