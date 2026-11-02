using MiniERP.Application.StockBalances.Dtos;
using MiniERP.Domain.Entities;

namespace MiniERP.Application.StockBalances;

public class StockBalanceService : IStockBalanceService
{
    private readonly IStockBalanceRepository _repository;

    public StockBalanceService(IStockBalanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StockBalanceDto>> GetAllAsync()
    {
        var balances = await _repository.GetAllAsync();

        return balances.Select(x => new StockBalanceDto
        {
            Id = x.Id,
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            SKU = x.Product.SKU,
            WarehouseId = x.WarehouseId,
            WarehouseName = x.Warehouse.Name,
            Quantity = x.Quantity,
            UpdatedAtUtc = x.UpdatedAtUtc
        }).ToList();
    }

    public async Task<List<StockMovementDto>> GetMovementsAsync()
    {
        var movements = await _repository.GetMovementsAsync();

        return movements.Select(x => new StockMovementDto
        {
            Id = x.Id,
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            SKU = x.Product.SKU,
            WarehouseId = x.WarehouseId,
            WarehouseName = x.Warehouse.Name,
            MovementType = x.MovementType,
            Quantity = x.Quantity,
            CreatedAtUtc = x.CreatedAtUtc,
            Description = x.Description
        }).ToList();
    }

    public async Task<StockBalanceDto> AddStockAsync(StockEntryDto dto)
    {
        if (dto.Quantity <= 0)
            throw new ArgumentException("Stok miktarı sıfırdan büyük olmalıdır.");

        if (!await _repository.ProductExistsAsync(dto.ProductId))
            throw new ArgumentException("Belirtilen ürün bulunamadı.");

        if (!await _repository.ProductIsActiveAsync(dto.ProductId))
        {
            throw new ArgumentException(
                "Pasif ürünlere stok girişi yapılamaz.");
        }

        if (!await _repository.WarehouseExistsAsync(dto.WarehouseId))
            throw new ArgumentException("Belirtilen depo bulunamadı.");

        if (!await _repository.WarehouseIsActiveAsync(dto.WarehouseId))
        {
            throw new ArgumentException(
                "Pasif depolarda stok girişi yapılamaz.");
        }

        var balance = await _repository.GetByProductAndWarehouseAsync(
            dto.ProductId,
            dto.WarehouseId);

        if (balance is null)
        {
            balance = new StockBalance
            {
                ProductId = dto.ProductId,
                WarehouseId = dto.WarehouseId,
                Quantity = dto.Quantity,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(balance);
        }
        else
        {
            balance.Quantity += dto.Quantity;
            balance.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _repository.AddMovementAsync(new StockMovement
        {
            ProductId = dto.ProductId,
            WarehouseId = dto.WarehouseId,
            MovementType = "StockEntry",
            Quantity = dto.Quantity,
            CreatedAtUtc = DateTime.UtcNow,
            Description = "Stok girişi"
        });

        await _repository.SaveChangesAsync();

        var balances = await _repository.GetAllAsync();
        var savedBalance = balances.First(x =>
            x.ProductId == dto.ProductId &&
            x.WarehouseId == dto.WarehouseId);

        return new StockBalanceDto
        {
            Id = savedBalance.Id,
            ProductId = savedBalance.ProductId,
            ProductName = savedBalance.Product.Name,
            SKU = savedBalance.Product.SKU,
            WarehouseId = savedBalance.WarehouseId,
            WarehouseName = savedBalance.Warehouse.Name,
            Quantity = savedBalance.Quantity,
            UpdatedAtUtc = savedBalance.UpdatedAtUtc
        };
    }

    public async Task<StockBalanceDto> RemoveStockAsync(StockExitDto dto)
    {
        if (dto.Quantity <= 0)
            throw new ArgumentException("Stok çıkış miktarı sıfırdan büyük olmalıdır.");

        if (!await _repository.ProductExistsAsync(dto.ProductId))
            throw new ArgumentException("Belirtilen ürün bulunamadı.");

        if (!await _repository.ProductIsActiveAsync(dto.ProductId))
        {
            throw new ArgumentException(
                "Pasif ürünlerden stok çıkışı yapılamaz.");
        }

        if (!await _repository.WarehouseExistsAsync(dto.WarehouseId))
            throw new ArgumentException("Belirtilen depo bulunamadı.");

        if (!await _repository.WarehouseIsActiveAsync(dto.WarehouseId))
        {
            throw new ArgumentException(
                "Pasif depolarda stok çıkışı yapılamaz.");
        }

        var balance = await _repository.GetByProductAndWarehouseAsync(
            dto.ProductId,
            dto.WarehouseId);

        if (balance is null)
            throw new ArgumentException(
                "Bu ürün için seçilen depoda stok kaydı bulunamadı.");

        if (balance.Quantity < dto.Quantity)
            throw new ArgumentException(
                $"Yetersiz stok. Mevcut miktar: {balance.Quantity}");

        balance.Quantity -= dto.Quantity;
        balance.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.AddMovementAsync(new StockMovement
        {
            ProductId = dto.ProductId,
            WarehouseId = dto.WarehouseId,
            MovementType = "StockExit",
            Quantity = dto.Quantity,
            CreatedAtUtc = DateTime.UtcNow,
            Description = "Stok çıkışı"
        });

        await _repository.SaveChangesAsync();

        var balances = await _repository.GetAllAsync();
        var savedBalance = balances.First(x =>
            x.ProductId == dto.ProductId &&
            x.WarehouseId == dto.WarehouseId);

        return new StockBalanceDto
        {
            Id = savedBalance.Id,
            ProductId = savedBalance.ProductId,
            ProductName = savedBalance.Product.Name,
            SKU = savedBalance.Product.SKU,
            WarehouseId = savedBalance.WarehouseId,
            WarehouseName = savedBalance.Warehouse.Name,
            Quantity = savedBalance.Quantity,
            UpdatedAtUtc = savedBalance.UpdatedAtUtc
        };
    }
}