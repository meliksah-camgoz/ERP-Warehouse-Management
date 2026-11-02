using MiniERP.Application.Assets.Dtos;
using MiniERP.Domain.Entities;
using ClosedXML.Excel;

namespace MiniERP.Application.Assets;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _repository;

    public AssetService(IAssetRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AssetDto>> GetAllAsync()
    {
        var assets = await _repository.GetAllAsync();

        return assets.Select(MapToDto).ToList();
    }

    public async Task<AssetDto?> GetByIdAsync(int id)
    {
        var asset = await _repository.GetByIdAsync(id);

        return asset is null ? null : MapToDto(asset);
    }

    public async Task<AssetDto> CreateAsync(CreateAssetDto dto)
    {
        var assetNumber = dto.AssetNumber.Trim();

        if (await _repository.AssetNumberExistsAsync(assetNumber))
        {
            throw new ArgumentException(
                "Bu demirbaş numarası zaten kullanılıyor.");
        }

        var now = DateTime.UtcNow;

        var asset = new Asset
        {
            AssetNumber = assetNumber,
            AssetType = dto.AssetType.Trim(),
            Brand = dto.Brand.Trim(),
            Model = dto.Model.Trim(),
            SerialNumber = Clean(dto.SerialNumber),
            AssignedTo = Clean(dto.AssignedTo),
            Department = Clean(dto.Department),
            Location = Clean(dto.Location),
            Status = string.IsNullOrWhiteSpace(dto.Status)
                ? "Kullanımda"
                : dto.Status.Trim(),
            WarrantyEndDate = dto.WarrantyEndDate,
            Description = Clean(dto.Description),
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        await _repository.AddAsync(asset);
        await _repository.SaveChangesAsync();

        return MapToDto(asset);
    }

    private static AssetDto MapToDto(Asset asset)
    {
        return new AssetDto
        {
            Id = asset.Id,
            AssetNumber = asset.AssetNumber,
            AssetType = asset.AssetType,
            Brand = asset.Brand,
            Model = asset.Model,
            SerialNumber = asset.SerialNumber,
            AssignedTo = asset.AssignedTo,
            Department = asset.Department,
            Location = asset.Location,
            Status = asset.Status,
            WarrantyEndDate = asset.WarrantyEndDate,
            Description = asset.Description,
            CreatedAtUtc = asset.CreatedAtUtc,
            UpdatedAtUtc = asset.UpdatedAtUtc
        };
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    public async Task<ImportAssetsResultDto> ImportExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();

        var result = new ImportAssetsResultDto();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        if (lastRow < 2)
        {
            result.Errors.Add("Excel dosyasında aktarılacak kayıt bulunamadı.");
            return result;
        }

        // Başlıkları bul: Türkçe ve İngilizce başlıkları destekle.
        var headerRow = worksheet.Row(1);
        var headers = new Dictionary<string, int>(
            StringComparer.OrdinalIgnoreCase);

        for (var column = 1; column <= (worksheet.LastColumnUsed()?.ColumnNumber() ?? 0); column++)
        {
            var header = NormalizeHeader(headerRow.Cell(column).GetString());

            if (!string.IsNullOrWhiteSpace(header) && !headers.ContainsKey(header))
                headers[header] = column;
        }

        int FindColumn(params string[] names)
        {
            foreach (var name in names)
            {
                var normalized = NormalizeHeader(name);
                if (headers.TryGetValue(normalized, out var column))
                    return column;
            }

            return 0;
        }

        var assetNumberCol = FindColumn("Demirbaş No", "AssetNumber");
        var assetTypeCol = FindColumn("Cihaz Türü", "AssetType");
        var brandCol = FindColumn("Marka", "Brand");
        var modelCol = FindColumn("Model");
        var serialCol = FindColumn("Seri No", "SerialNumber");
        var assignedCol = FindColumn("Zimmetli Personel", "AssignedTo");
        var departmentCol = FindColumn("Departman", "Department");
        var locationCol = FindColumn("Konum", "Location");
        var statusCol = FindColumn("Durum", "Status");
        var warrantyCol = FindColumn("Garanti Bitiş Tarihi", "WarrantyEndDate");
        var descriptionCol = FindColumn("Açıklama", "Description");

        var requiredHeaders = new Dictionary<string, int>
        {
            ["Demirbaş No"] = assetNumberCol,
            ["Cihaz Türü"] = assetTypeCol,
            ["Marka"] = brandCol,
            ["Model"] = modelCol
        };

        var missingHeaders = requiredHeaders
            .Where(x => x.Value == 0)
            .Select(x => x.Key)
            .ToList();

        if (missingHeaders.Count > 0)
        {
            result.Errors.Add(
                "Eksik zorunlu sütunlar: " + string.Join(", ", missingHeaders));
            return result;
        }

        var now = DateTime.UtcNow;
        var seenAssetNumbers = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        for (var row = 2; row <= lastRow; row++)
        {
            var assetNumber = worksheet.Cell(row, assetNumberCol).GetString().Trim();
            var assetType = worksheet.Cell(row, assetTypeCol).GetString().Trim();
            var brand = worksheet.Cell(row, brandCol).GetString().Trim();
            var model = worksheet.Cell(row, modelCol).GetString().Trim();

            // Tamamen boş satırları atla.
            if (string.IsNullOrWhiteSpace(assetNumber) &&
                string.IsNullOrWhiteSpace(assetType) &&
                string.IsNullOrWhiteSpace(brand) &&
                string.IsNullOrWhiteSpace(model))
            {
                continue;
            }

            result.TotalRows++;

            if (string.IsNullOrWhiteSpace(assetNumber) ||
                string.IsNullOrWhiteSpace(assetType) ||
                string.IsNullOrWhiteSpace(brand) ||
                string.IsNullOrWhiteSpace(model))
            {
                result.FailedCount++;
                result.Errors.Add(
                    $"Satır {row}: Demirbaş No, Cihaz Türü, Marka ve Model zorunludur.");
                continue;
            }

            if (!seenAssetNumbers.Add(assetNumber))
            {
                result.FailedCount++;
                result.Errors.Add(
                    $"Satır {row}: {assetNumber} aynı Excel dosyasında birden fazla kez kullanılmış.");
                continue;
            }

            if (await _repository.AssetNumberExistsAsync(assetNumber))
            {
                result.FailedCount++;
                result.Errors.Add(
                    $"Satır {row}: {assetNumber} demirbaş numarası zaten kayıtlı.");
                continue;
            }

            var asset = new Asset
            {
                AssetNumber = assetNumber,
                AssetType = assetType,
                Brand = brand,
                Model = model,
                SerialNumber = ReadText(worksheet, row, serialCol),
                AssignedTo = ReadText(worksheet, row, assignedCol),
                Department = ReadText(worksheet, row, departmentCol),
                Location = ReadText(worksheet, row, locationCol),
                Status = ReadText(worksheet, row, statusCol) ?? "Kullanımda",
                WarrantyEndDate = ReadDate(worksheet, row, warrantyCol),
                Description = ReadText(worksheet, row, descriptionCol),
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            await _repository.AddAsync(asset);

            try
            {
                await _repository.SaveChangesAsync();
                result.ImportedCount++;
            }
            catch (Exception)
            {
                result.FailedCount++;
                result.Errors.Add(
                    $"Satır {row}: Kayıt veritabanına kaydedilemedi. Demirbaş numarasını kontrol edin.");
            }
        }

        return result;
    }

    private static string NormalizeHeader(string value)
    {
        return value.Trim()
            .Replace("ı", "i")
            .Replace("İ", "i")
            .Replace("I", "i")
            .ToLowerInvariant();
    }

    private static string? ReadText(IXLWorksheet worksheet, int row, int column)
    {
        if (column == 0)
            return null;

        return Clean(worksheet.Cell(row, column).GetString());
    }

    private static DateTime? ReadDate(IXLWorksheet worksheet, int row, int column)
    {
        if (column == 0)
            return null;

        var cell = worksheet.Cell(row, column);

        if (cell.IsEmpty())
            return null;

        if (cell.TryGetValue<DateTime>(out var date))
            return date;

        return DateTime.TryParse(cell.GetString(), out var parsed)
            ? parsed
            : null;
    }

}