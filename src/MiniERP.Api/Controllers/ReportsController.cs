
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniERP.Infrastructure.Persistence;

namespace MiniERP.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly MiniERPDbContext _context;

    public ReportsController(MiniERPDbContext context)
    {
        _context = context;
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportExcel(
        CancellationToken cancellationToken)
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var stockBalances = await _context.StockBalances
            .AsNoTracking()
            .Include(x => x.Product)
                .ThenInclude(x => x!.Category)
            .Include(x => x.Warehouse)
            .OrderBy(x => x.Warehouse.Name)
            .ThenBy(x => x.Product.Name)
            .ToListAsync(cancellationToken);

        var movements = await _context.StockMovements
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var assets = await _context.Assets
            .AsNoTracking()
            .OrderBy(x => x.AssetNumber)
            .ToListAsync(cancellationToken);

        var warehouseCount = await _context.Warehouses
            .CountAsync(cancellationToken);

        // Ürünlerin bütün depolardaki toplam stok miktarları
        var productStockTotals = stockBalances
            .GroupBy(x => x.ProductId)
            .ToDictionary(
                x => x.Key,
                x => x.Sum(s => s.Quantity));

        decimal GetTotalStock(int productId)
        {
            return productStockTotals.TryGetValue(productId, out var quantity)
                ? quantity
                : 0;
        }

        using var workbook = new XLWorkbook();

        // =====================================================
        // 1. YÖNETİCİ ÖZETİ
        // =====================================================

        var summary = workbook.Worksheets.Add("Yönetici Özeti");

        summary.Cell("A1").Value = "MINIERP - KURUMSAL RAPOR";
        summary.Range("A1:F1").Merge();

        summary.Cell("A2").Value = "Rapor Tarihi";
        summary.Cell("B2").Value = DateTime.Now;

        summary.Cell("A4").Value = "GENEL GÖSTERGELER";

        summary.Cell("A5").Value = "Toplam Ürün";
        summary.Cell("B5").Value = products.Count;

        summary.Cell("A6").Value = "Aktif Ürün";
        summary.Cell("B6").Value = products.Count(x => x.IsActive);

        summary.Cell("A7").Value = "Pasif Ürün";
        summary.Cell("B7").Value = products.Count(x => !x.IsActive);

        summary.Cell("A8").Value = "Toplam Depo";
        summary.Cell("B8").Value = warehouseCount;

        summary.Cell("A9").Value = "Toplam Stok Kaydı";
        summary.Cell("B9").Value = stockBalances.Count;

        summary.Cell("A10").Value = "Toplam Stok Miktarı";
        summary.Cell("B10").Value = stockBalances.Sum(x => x.Quantity);

        summary.Cell("A11").Value = "Kritik Stok Ürün Sayısı";
        summary.Cell("B11").Value = products.Count(p =>
            GetTotalStock(p.Id) <= p.MinStockLevel);

        summary.Cell("A12").Value = "Toplam Stok Maliyeti";
        summary.Cell("B12").Value = stockBalances.Sum(x =>
            x.Quantity * x.Product.PurchasePrice);

        summary.Cell("A13").Value = "Toplam Stok Satış Değeri";
        summary.Cell("B13").Value = stockBalances.Sum(x =>
            x.Quantity * x.Product.SalePrice);

        summary.Cell("A14").Value = "Toplam Stok Hareketi";
        summary.Cell("B14").Value = movements.Count;

        summary.Cell("A15").Value = "Toplam Demirbaş";
        summary.Cell("B15").Value = assets.Count;

        summary.Cell("B2").Style.DateFormat.Format =
            "dd.MM.yyyy HH:mm";

        summary.Range("B10:B10").Style.NumberFormat.Format = "#,##0.00";
        summary.Range("B12:B13").Style.NumberFormat.Format = "#,##0.00";

        // ÜRÜN BAZINDA STOK DURUMU TABLOSU
        summary.Cell("A18").Value = "ÜRÜN BAZINDA STOK DURUMU";
        summary.Range("A18:F18").Merge();

        var summaryHeaders = new[]
        {
            "Ürün Kodu",
            "Ürün Adı",
            "Kategori",
            "Toplam Stok",
            "Minimum Stok",
            "Stok Durumu"
        };

        for (int i = 0; i < summaryHeaders.Length; i++)
        {
            summary.Cell(19, i + 1).Value = summaryHeaders[i];
        }

        for (int i = 0; i < products.Count; i++)
        {
            var p = products[i];
            var row = i + 20;
            var totalStock = GetTotalStock(p.Id);

            summary.Cell(row, 1).Value = p.SKU;
            summary.Cell(row, 2).Value = p.Name;
            summary.Cell(row, 3).Value = p.Category?.Name;
            summary.Cell(row, 4).Value = totalStock;
            summary.Cell(row, 5).Value = p.MinStockLevel;

            summary.Cell(row, 6).Value =
                totalStock <= 0 ? "Stok Yok" :
                totalStock <= p.MinStockLevel ? "Kritik" :
                "Normal";

            if (totalStock <= 0)
            {
                summary.Cell(row, 6).Style.Font.FontColor =
                    XLColor.DarkRed;
            }
            else if (totalStock <= p.MinStockLevel)
            {
                summary.Cell(row, 6).Style.Font.FontColor =
                    XLColor.DarkOrange;
            }
            else
            {
                summary.Cell(row, 6).Style.Font.FontColor =
                    XLColor.DarkGreen;
            }
        }

        summary.Range(20, 4, Math.Max(20, products.Count + 19), 5)
            .Style.NumberFormat.Format = "#,##0.00";

        if (products.Count > 0)
        {
            summary.Range(19, 1, products.Count + 19, 6)
                .SetAutoFilter();
        }

        // =====================================================
        // 2. ÜRÜN LİSTESİ
        // =====================================================

        var productSheet = workbook.Worksheets.Add("Ürün Listesi");

        var productHeaders = new[]
        {
            "ID",
            "Ürün Kodu",
            "Barkod",
            "Ürün Adı",
            "Kategori",
            "Birim",
            "Toplam Stok",
            "Alış Fiyatı",
            "Satış Fiyatı",
            "KDV Oranı",
            "Minimum Stok",
            "Durum",
            "Kayıt Tarihi"
        };

        for (int i = 0; i < productHeaders.Length; i++)
        {
            productSheet.Cell(1, i + 1).Value = productHeaders[i];
        }

        for (int i = 0; i < products.Count; i++)
        {
            var p = products[i];
            var row = i + 2;

            productSheet.Cell(row, 1).Value = p.Id;
            productSheet.Cell(row, 2).Value = p.SKU;
            productSheet.Cell(row, 3).Value = p.Barcode;
            productSheet.Cell(row, 4).Value = p.Name;
            productSheet.Cell(row, 5).Value = p.Category?.Name;
            productSheet.Cell(row, 6).Value = p.Unit;
            productSheet.Cell(row, 7).Value = GetTotalStock(p.Id);
            productSheet.Cell(row, 8).Value = p.PurchasePrice;
            productSheet.Cell(row, 9).Value = p.SalePrice;
            productSheet.Cell(row, 10).Value = p.VatRate;
            productSheet.Cell(row, 11).Value = p.MinStockLevel;
            productSheet.Cell(row, 12).Value =
                p.IsActive ? "Aktif" : "Pasif";
            productSheet.Cell(row, 13).Value = p.CreatedAtUtc;
        }

        productSheet.Column(7).Style.NumberFormat.Format = "#,##0.00";
        productSheet.Columns(8, 9).Style.NumberFormat.Format = "#,##0.00";
        productSheet.Column(10).Style.NumberFormat.Format = "0.00";
        productSheet.Column(11).Style.NumberFormat.Format = "#,##0.00";
        productSheet.Column(13).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";

        // =====================================================
        // 3. DEPO STOKLARI
        // =====================================================

        var stockSheet = workbook.Worksheets.Add("Depo Stokları");

        var stockHeaders = new[]
        {
            "Ürün Kodu",
            "Barkod",
            "Ürün Adı",
            "Kategori",
            "Depo Kodu",
            "Depo Adı",
            "Stok Miktarı",
            "Birim",
            "Minimum Stok",
            "Alış Fiyatı",
            "Stok Maliyeti",
            "Stok Satış Değeri",
            "Depo Stok Durumu"
        };

        for (int i = 0; i < stockHeaders.Length; i++)
        {
            stockSheet.Cell(1, i + 1).Value = stockHeaders[i];
        }

        for (int i = 0; i < stockBalances.Count; i++)
        {
            var s = stockBalances[i];
            var p = s.Product;
            var w = s.Warehouse;
            var row = i + 2;

            stockSheet.Cell(row, 1).Value = p.SKU;
            stockSheet.Cell(row, 2).Value = p.Barcode;
            stockSheet.Cell(row, 3).Value = p.Name;
            stockSheet.Cell(row, 4).Value = p.Category?.Name;
            stockSheet.Cell(row, 5).Value = w.Code;
            stockSheet.Cell(row, 6).Value = w.Name;
            stockSheet.Cell(row, 7).Value = s.Quantity;
            stockSheet.Cell(row, 8).Value = p.Unit;
            stockSheet.Cell(row, 9).Value = p.MinStockLevel;
            stockSheet.Cell(row, 10).Value = p.PurchasePrice;
            stockSheet.Cell(row, 11).Value =
                s.Quantity * p.PurchasePrice;
            stockSheet.Cell(row, 12).Value =
                s.Quantity * p.SalePrice;

            stockSheet.Cell(row, 13).Value =
                s.Quantity <= 0 ? "Stok Yok" :
                s.Quantity <= p.MinStockLevel ? "Kritik" :
                "Normal";

            if (s.Quantity <= 0)
            {
                stockSheet.Cell(row, 13).Style.Font.FontColor =
                    XLColor.DarkRed;
            }
            else if (s.Quantity <= p.MinStockLevel)
            {
                stockSheet.Cell(row, 13).Style.Font.FontColor =
                    XLColor.DarkOrange;
            }
            else
            {
                stockSheet.Cell(row, 13).Style.Font.FontColor =
                    XLColor.DarkGreen;
            }
        }

        stockSheet.Column(7).Style.NumberFormat.Format = "#,##0.00";
        stockSheet.Column(9).Style.NumberFormat.Format = "#,##0.00";
        stockSheet.Columns(10, 12).Style.NumberFormat.Format = "#,##0.00";

        // =====================================================
        // 4. STOK HAREKETLERİ
        // =====================================================

        var movementSheet = workbook.Worksheets.Add("Stok Hareketleri");

        var movementHeaders = new[]
        {
            "Hareket ID",
            "Ürün Kodu",
            "Ürün Adı",
            "Depo Kodu",
            "Depo Adı",
            "Hareket Türü",
            "Miktar",
            "Açıklama",
            "İşlem Tarihi"
        };

        for (int i = 0; i < movementHeaders.Length; i++)
        {
            movementSheet.Cell(1, i + 1).Value = movementHeaders[i];
        }

        for (int i = 0; i < movements.Count; i++)
        {
            var m = movements[i];
            var row = i + 2;

            movementSheet.Cell(row, 1).Value = m.Id;
            movementSheet.Cell(row, 2).Value = m.Product.SKU;
            movementSheet.Cell(row, 3).Value = m.Product.Name;
            movementSheet.Cell(row, 4).Value = m.Warehouse.Code;
            movementSheet.Cell(row, 5).Value = m.Warehouse.Name;
            movementSheet.Cell(row, 6).Value = m.MovementType;
            movementSheet.Cell(row, 7).Value = m.Quantity;
            movementSheet.Cell(row, 8).Value = m.Description;
            movementSheet.Cell(row, 9).Value = m.CreatedAtUtc;
        }

        movementSheet.Column(7).Style.NumberFormat.Format = "#,##0.00";
        movementSheet.Column(9).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";

        // =====================================================
        // 5. KRİTİK STOKLAR
        // =====================================================

        var criticalSheet = workbook.Worksheets.Add("Kritik Stoklar");

        var criticalHeaders = new[]
        {
            "Ürün Kodu",
            "Ürün Adı",
            "Kategori",
            "Toplam Stok",
            "Minimum Stok",
            "Eksik Miktar",
            "Birim",
            "Durum"
        };

        for (int i = 0; i < criticalHeaders.Length; i++)
        {
            criticalSheet.Cell(1, i + 1).Value = criticalHeaders[i];
        }

        var criticalProducts = products
            .Select(p => new
            {
                Product = p,
                Quantity = GetTotalStock(p.Id)
            })
            .Where(x => x.Quantity <= x.Product.MinStockLevel)
            .OrderBy(x => x.Quantity)
            .ToList();

        for (int i = 0; i < criticalProducts.Count; i++)
        {
            var item = criticalProducts[i];
            var p = item.Product;
            var row = i + 2;

            criticalSheet.Cell(row, 1).Value = p.SKU;
            criticalSheet.Cell(row, 2).Value = p.Name;
            criticalSheet.Cell(row, 3).Value = p.Category?.Name;
            criticalSheet.Cell(row, 4).Value = item.Quantity;
            criticalSheet.Cell(row, 5).Value = p.MinStockLevel;
            criticalSheet.Cell(row, 6).Value =
                Math.Max(0, p.MinStockLevel - item.Quantity);
            criticalSheet.Cell(row, 7).Value = p.Unit;
            criticalSheet.Cell(row, 8).Value =
                item.Quantity <= 0 ? "Stok Yok" : "Kritik";
        }

        criticalSheet.Columns(4, 6).Style.NumberFormat.Format = "#,##0.00";

        // =====================================================
        // 6. DEMİRBAŞLAR
        // =====================================================

        var assetSheet = workbook.Worksheets.Add("Demirbaşlar");

        var assetHeaders = new[]
        {
            "Demirbaş No",
            "Tür",
            "Marka",
            "Model",
            "Seri No",
            "Zimmetli Kişi",
            "Departman",
            "Konum",
            "Durum",
            "Garanti Bitiş",
            "Açıklama",
            "Kayıt Tarihi"
        };

        for (int i = 0; i < assetHeaders.Length; i++)
        {
            assetSheet.Cell(1, i + 1).Value = assetHeaders[i];
        }

        for (int i = 0; i < assets.Count; i++)
        {
            var a = assets[i];
            var row = i + 2;

            assetSheet.Cell(row, 1).Value = a.AssetNumber;
            assetSheet.Cell(row, 2).Value = a.AssetType;
            assetSheet.Cell(row, 3).Value = a.Brand;
            assetSheet.Cell(row, 4).Value = a.Model;
            assetSheet.Cell(row, 5).Value = a.SerialNumber;
            assetSheet.Cell(row, 6).Value = a.AssignedTo;
            assetSheet.Cell(row, 7).Value = a.Department;
            assetSheet.Cell(row, 8).Value = a.Location;
            assetSheet.Cell(row, 9).Value = a.Status;
            assetSheet.Cell(row, 10).Value = a.WarrantyEndDate;
            assetSheet.Cell(row, 11).Value = a.Description;
            assetSheet.Cell(row, 12).Value = a.CreatedAtUtc;
        }

        assetSheet.Column(10).Style.DateFormat.Format = "dd.MM.yyyy";
        assetSheet.Column(12).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";

        // =====================================================
        // ORTAK SAYFA BİÇİMLENDİRMESİ
        // =====================================================

        foreach (var sheet in workbook.Worksheets)
        {
            sheet.SheetView.FreezeRows(1);

            var usedRange = sheet.RangeUsed();

            if (usedRange == null)
                continue;

            sheet.Columns().AdjustToContents();

            var header = sheet.Row(1);

            header.Style.Font.Bold = true;
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#17365D");

            header.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            header.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            header.Height = 28;

            // Yönetici Özeti'nde filtre yalnızca ürün tablosuna uygulanıyor.
            if (sheet.Name != "Yönetici Özeti" &&
                usedRange.RowCount() > 1)
            {
                usedRange.SetAutoFilter();
            }

            if (sheet.Name != "Yönetici Özeti" &&
                usedRange.RowCount() > 1)
            {
                for (int row = 2; row <= usedRange.RowCount(); row++)
                {
                    if (row % 2 == 0)
                    {
                        sheet.Row(row).Style.Fill.BackgroundColor =
                            XLColor.FromHtml("#F2F6FA");
                    }
                }
            }
        }

        // Yönetici Özeti özel tasarım
        summary.Range("A1:F1").Style.Font.Bold = true;
        summary.Range("A1:F1").Style.Font.FontSize = 16;
        summary.Range("A1:F1").Style.Font.FontColor = XLColor.White;
        summary.Range("A1:F1").Style.Fill.BackgroundColor =
            XLColor.FromHtml("#17365D");

        summary.Range("A4:B4").Merge();
        summary.Range("A4:B4").Style.Font.Bold = true;
        summary.Range("A4:B4").Style.Fill.BackgroundColor =
            XLColor.FromHtml("#D9EAF7");

        summary.Range("A18:F18").Style.Font.Bold = true;
        summary.Range("A18:F18").Style.Font.FontColor = XLColor.White;
        summary.Range("A18:F18").Style.Fill.BackgroundColor =
            XLColor.FromHtml("#17365D");

        var summaryTableHeader = summary.Range("A19:F19");

        summaryTableHeader.Style.Font.Bold = true;
        summaryTableHeader.Style.Font.FontColor = XLColor.White;
        summaryTableHeader.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#4472C4");

        summaryTableHeader.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        summaryTableHeader.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        summary.Row(19).Height = 25;

        summary.Column(1).Width = 22;
        summary.Column(2).Width = 30;
        summary.Column(3).Width = 22;
        summary.Column(4).Width = 16;
        summary.Column(5).Width = 16;
        summary.Column(6).Width = 18;

        if (products.Count > 0)
        {
            for (int row = 20; row < products.Count + 20; row++)
            {
                if (row % 2 == 0)
                {
                    summary.Range(row, 1, row, 6).Style.Fill.BackgroundColor =
                        XLColor.FromHtml("#F2F6FA");
                }
            }
        }

        // Dosyayı oluştur
        using var stream = new MemoryStream();

        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName =
            $"MiniERP_Kurumsal_Rapor_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
