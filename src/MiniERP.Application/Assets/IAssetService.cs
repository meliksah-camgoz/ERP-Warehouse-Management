using MiniERP.Application.Assets.Dtos;

namespace MiniERP.Application.Assets;

public interface IAssetService
{
    Task<List<AssetDto>> GetAllAsync();

    Task<AssetDto?> GetByIdAsync(int id);

    Task<AssetDto> CreateAsync(CreateAssetDto dto);

    Task<ImportAssetsResultDto> ImportExcelAsync(Stream fileStream);
}