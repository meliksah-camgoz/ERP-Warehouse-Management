using Microsoft.EntityFrameworkCore;
using MiniERP.Application.Assets;
using MiniERP.Domain.Entities;
using MiniERP.Infrastructure.Persistence;

namespace MiniERP.Infrastructure.Assets;

public class AssetRepository : IAssetRepository
{
    private readonly MiniERPDbContext _context;

    public AssetRepository(MiniERPDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asset>> GetAllAsync()
    {
        return await _context.Assets
            .AsNoTracking()
            .OrderBy(x => x.AssetNumber)
            .ToListAsync();
    }

    public async Task<Asset?> GetByIdAsync(int id)
    {
        return await _context.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<bool> AssetNumberExistsAsync(string assetNumber)
    {
        return _context.Assets.AnyAsync(
            x => x.AssetNumber == assetNumber);
    }

    public async Task AddAsync(Asset asset)
    {
        await _context.Assets.AddAsync(asset);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}