using Microsoft.EntityFrameworkCore;
using MiniERP.Application.Products;
using MiniERP.Domain.Entities;
using MiniERP.Infrastructure.Persistence;

namespace MiniERP.Infrastructure.Products;

public class ProductRepository : IProductRepository
{
    private readonly MiniERPDbContext _context;

    public ProductRepository(MiniERPDbContext context)
    {
        _context = context;
    }

    public Task<List<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Product>> GetActiveAsync(
    CancellationToken cancellationToken = default)
    {
        return _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<bool> SkuExistsAsync(
        string sku,
        int? excludeProductId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.Where(p => p.SKU == sku);

        if (excludeProductId.HasValue)
            query = query.Where(p => p.Id != excludeProductId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> CategoryExistsAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        return _context.Categories.AnyAsync(
            c => c.Id == categoryId && c.IsActive,
            cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }


    public async Task DeleteAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Önce ürünün stok hareketlerini sil.
            await _context.StockMovements
                .Where(x => x.ProductId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            // Ardından depo stok bakiyelerini sil.
            await _context.StockBalances
                .Where(x => x.ProductId == product.Id)
                .ExecuteDeleteAsync(cancellationToken);

            // Son olarak ürünü tamamen sil.
            _context.Products.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }



    public Task<bool> HasStockBalancesAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return _context.StockBalances
            .AnyAsync(
                x => x.ProductId == productId && x.Quantity > 0,
                cancellationToken);
    }


    public Task<bool> HasStockMovementsAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return _context.StockMovements
            .AnyAsync(x => x.ProductId == productId, cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}