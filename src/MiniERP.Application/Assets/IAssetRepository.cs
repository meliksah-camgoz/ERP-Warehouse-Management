using MiniERP.Domain.Entities;

namespace MiniERP.Application.Assets;

public interface IAssetRepository
{
    Task<List<Asset>> GetAllAsync();

    Task<Asset?> GetByIdAsync(int id);

    Task<bool> AssetNumberExistsAsync(string assetNumber);

    Task AddAsync(Asset asset);

    Task SaveChangesAsync();
}