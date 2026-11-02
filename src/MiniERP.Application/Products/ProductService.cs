using MiniERP.Application.Products.Dtos;
using MiniERP.Domain.Entities;

namespace MiniERP.Application.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var sku = NormalizeSku(dto.SKU);

        if (!await _repository.CategoryExistsAsync(
                dto.CategoryId, cancellationToken))
            throw new ArgumentException("Kategori bulunamadı.");

        if (await _repository.SkuExistsAsync(
                sku, cancellationToken: cancellationToken))
            throw new ArgumentException("Bu ürün kodu zaten kullanılıyor.");

        var product = new Product
        {
            CategoryId = dto.CategoryId,
            SKU = sku,
            Barcode = NormalizeOptional(dto.Barcode),
            Name = dto.Name.Trim(),
            Unit = dto.Unit.Trim(),
            PurchasePrice = dto.PurchasePrice,
            SalePrice = dto.SalePrice,
            VatRate = dto.VatRate,
            MinStockLevel = dto.MinStockLevel,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);

        if (product is null || !product.IsActive)
            return false;

        var sku = NormalizeSku(dto.SKU);

        if (!await _repository.CategoryExistsAsync(
                dto.CategoryId, cancellationToken))
            throw new ArgumentException("Kategori bulunamadı.");

        if (await _repository.SkuExistsAsync(
                sku, id, cancellationToken))
            throw new ArgumentException("Bu ürün kodu başka bir üründe kullanılıyor.");

        product.CategoryId = dto.CategoryId;
        product.SKU = sku;
        product.Barcode = NormalizeOptional(dto.Barcode);
        product.Name = dto.Name.Trim();
        product.Unit = dto.Unit.Trim();
        product.PurchasePrice = dto.PurchasePrice;
        product.SalePrice = dto.SalePrice;
        product.VatRate = dto.VatRate;
        product.MinStockLevel = dto.MinStockLevel;

        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(
            id, cancellationToken);

        if (product is null)
            return false;

        if (await _repository.HasStockBalancesAsync(
                id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Stok miktarı sıfırdan büyük olan ürün silinemez.");
        }

        product.IsActive = false;

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string NormalizeSku(string sku) =>
        sku.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            SKU = product.SKU,
            Barcode = product.Barcode,
            Name = product.Name,
            Unit = product.Unit,
            PurchasePrice = product.PurchasePrice,
            SalePrice = product.SalePrice,
            VatRate = product.VatRate,
            MinStockLevel = product.MinStockLevel,
            IsActive = product.IsActive,
            CreatedAtUtc = product.CreatedAtUtc
        };
    }
}