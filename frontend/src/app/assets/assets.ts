
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface Asset {
  id: number;
  assetNumber: string;
  assetType: string;
  brand: string;
  model: string;
  serialNumber?: string | null;
  assignedTo?: string | null;
  department?: string | null;
  location?: string | null;
  status: string;
  warrantyEndDate?: string | null;
  description?: string | null;
  createdAtUtc: string;
  updatedAtUtc: string;
}

interface AssetForm {
  assetNumber: string;
  assetType: string;
  brand: string;
  model: string;
  serialNumber: string;
  assignedTo: string;
  department: string;
  location: string;
  status: string;
  warrantyEndDate: string;
  description: string;
}
interface ProductOption {
  id: number;
  sku: string;
  name: string;
  unit: string;
  isActive: boolean;
}

interface WarehouseOption {
  id: number;
  code: string;
  name: string;
  isActive: boolean;
}

interface StockBalanceOption {
  productId: number;
  warehouseId: number;
  productName: string;
  sku: string;
  warehouseName: string;
  quantity: number;
}

@Component({
  selector: 'app-assets',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './assets.html',
  styleUrl: './assets.css'
})
export class Assets implements OnInit {
  private readonly apiUrl = '/api/assets';

  assets: Asset[] = [];
  products: ProductOption[] = [];
  warehouses: WarehouseOption[] = [];
  stockBalances: StockBalanceOption[] = [];

  selectedProductId: number | null = null;
  selectedWarehouseId: number | null = null;
  filteredAssets: Asset[] = [];
  loading = false;
  saving = false;
  showForm = false;
  errorMessage = '';
  successMessage = '';
  searchText = '';
  statusFilter = 'TÃ¼mÃ¼';
  selectedFile: File | null = null;

  form: AssetForm = this.emptyForm();

  constructor(
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadAssets();
    this.loadProducts();
    this.loadWarehouses();
    this.loadStockBalances();
  }

  private emptyForm(): AssetForm {
    return {
      assetNumber: '',
      assetType: '',
      brand: '',
      model: '',
      serialNumber: '',
      assignedTo: '',
      department: '',
      location: '',
      status: 'KullanÄ±mda',
      warrantyEndDate: '',
      description: ''
    };
  }

  loadAssets(): void {
    this.loading = true;
    this.errorMessage = '';

    this.http.get<Asset[]>(this.apiUrl).subscribe({
      next: (data) => {
        this.assets = data ?? [];
        this.applyFilters();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || 'Envanter kayÄ±tlarÄ± yÃ¼klenemedi. API Ã§alÄ±ÅŸÄ±yor mu?';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  applyFilters(): void {
    const search = this.searchText.trim().toLocaleLowerCase('tr-TR');

    this.filteredAssets = this.assets.filter((asset) => {
      const matchesSearch = [
        asset.assetNumber,
        asset.assetType,
        asset.brand,
        asset.model,
        asset.serialNumber,
        asset.assignedTo,
        asset.department,
        asset.location
      ].some((value) =>
        (value ?? '').toLocaleLowerCase('tr-TR').includes(search)
      );

      const matchesStatus =
        this.statusFilter === 'TÃ¼mÃ¼' || asset.status === this.statusFilter;

      return matchesSearch && matchesStatus;
    });
  }

  openForm(): void {
    this.form = this.emptyForm();
    this.errorMessage = '';
    this.successMessage = '';
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
  }

  saveAsset(): void {
    if (
      !this.form.assetNumber.trim() ||
      !this.form.assetType.trim() ||
      !this.form.brand.trim() ||
      !this.form.model.trim()
    ) {
      this.errorMessage = 'DemirbaÅŸ numarasÄ±, cihaz tÃ¼rÃ¼, marka ve model zorunludur.';
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    const payload = {
      ...this.form,
      assetNumber: this.form.assetNumber.trim(),
      assetType: this.form.assetType.trim(),
      brand: this.form.brand.trim(),
      model: this.form.model.trim(),
      serialNumber: this.form.serialNumber.trim() || null,
      assignedTo: this.form.assignedTo.trim() || null,
      department: this.form.department.trim() || null,
      location: this.form.location.trim() || null,
      warrantyEndDate: this.form.warrantyEndDate || null,
      description: this.form.description.trim() || null
    };

    this.http.post<Asset>(this.apiUrl, payload).subscribe({
      next: () => {
        this.saving = false;
        this.showForm = false;
        this.successMessage = 'Envanter kaydÄ± baÅŸarÄ±yla oluÅŸturuldu.';
        this.loadAssets();
      },
      error: (error) => {
        this.saving = false;
        this.errorMessage =
          error.error?.message || 'KayÄ±t oluÅŸturulamadÄ±.';
        this.cdr.detectChanges();
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  importExcel(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Ã–nce bir Excel (.xlsx) dosyasÄ± seÃ§.';
      return;
    }

    const data = new FormData();
    data.append('file', this.selectedFile);

    this.errorMessage = '';
    this.successMessage = '';

    this.http.post<any>(`${this.apiUrl}/import-excel`, data).subscribe({
      next: (result) => {
        this.successMessage =
          result?.message || 'Excel iÃ§e aktarma iÅŸlemi tamamlandÄ±.';
        this.selectedFile = null;
        this.loadAssets();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || 'Excel dosyasÄ± iÃ§e aktarÄ±lamadÄ±.';
        this.cdr.detectChanges();
      }
    });
  }

  loadProducts(): void {
    this.http.get<ProductOption[]>('/api/products')
      .subscribe({
        next: (data) => {
          this.products = (data ?? []).filter(p => p.isActive);
          this.cdr.detectChanges();
        },
        error: () => {
          this.errorMessage = 'ÃœrÃ¼n listesi yÃ¼klenemedi.';
          this.cdr.detectChanges();
        }
      });
  }

  loadWarehouses(): void {
    this.http.get<WarehouseOption[]>('/api/warehouses')
      .subscribe({
        next: (data) => {
          this.warehouses = (data ?? []).filter(w => w.isActive);
          this.cdr.detectChanges();
        },
        error: () => {
          this.errorMessage = 'Depo listesi yÃ¼klenemedi.';
          this.cdr.detectChanges();
        }
      });
  }

  loadStockBalances(): void {
    this.http.get<StockBalanceOption[]>('/api/stock-balances')
      .subscribe({
        next: (data) => {
          this.stockBalances = data ?? [];
          this.cdr.detectChanges();
        },
        error: () => {
          this.errorMessage = 'Stok bilgileri yÃ¼klenemedi.';
          this.cdr.detectChanges();
        }
      });
  }

  get selectedStockQuantity(): number {
    if (this.selectedProductId === null ||
      this.selectedWarehouseId === null) {
      return 0;
    }

    return this.stockBalances.find(
      b => b.productId === this.selectedProductId &&
        b.warehouseId === this.selectedWarehouseId
    )?.quantity ?? 0;
  }

  get totalCount(): number {
    return this.assets.length;
  }

  get assignedCount(): number {
    return this.assets.filter((a) => !!a.assignedTo?.trim()).length;
  }

  get availableCount(): number {
    return this.assets.filter((a) =>
      ['Stokta', 'KullanÄ±labilir', 'BoÅŸta'].includes(a.status)
    ).length;
  }

  get maintenanceCount(): number {
    return this.assets.filter((a) =>
      ['ArÄ±zalÄ±', 'BakÄ±mda', 'Serviste'].includes(a.status)
    ).length;
  }

  trackById(_index: number, asset: Asset): number {
    return asset.id;
  }
}

