using Microsoft.EntityFrameworkCore;
using MiniERP.Application.Categories;
using MiniERP.Domain.Entities;
using MiniERP.Infrastructure.Persistence;

namespace MiniERP.Infrastructure.Categories;

public class CategoryRepository : ICategoryRepository
{
    private readonly MiniERPDbContext _context;

    public CategoryRepository(MiniERPDbContext context)
    {
        _context = context;
    }

    public Task<List<Category>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}