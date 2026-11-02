
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';

interface Warehouse {
  id: number;
  code: string;
  name: string;
  address: string | null;
  isActive: boolean;
  createdAtUtc: string;
}

interface WarehouseForm {
  code: string;
  name: string;
  address: string;
}

@Component({
  selector: 'app-warehouses',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './warehouses.html',
  styleUrl: './warehouses.css',
})
export class Warehouses implements OnInit {
  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);
  private apiUrl = 'http://localhost:5252/api/warehouses';

  warehouses: Warehouse[] = [];
  searchTerm = '';
  statusFilter = 'all';

  isLoading = false;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  showForm = false;
  editingId: number | null = null;

  form: WarehouseForm = this.emptyForm();

  ngOnInit(): void {
    this.loadWarehouses();
  }

  private emptyForm(): WarehouseForm {
    return { code: '', name: '', address: '' };
  }

  get filteredWarehouses(): Warehouse[] {
    const term = this.searchTerm.trim().toLocaleLowerCase('tr-TR');

    return this.warehouses.filter((warehouse) => {
      const matchesSearch =
        !term ||
        warehouse.code.toLocaleLowerCase('tr-TR').includes(term) ||
        warehouse.name.toLocaleLowerCase('tr-TR').includes(term) ||
        (warehouse.address ?? '').toLocaleLowerCase('tr-TR').includes(term);

      const matchesStatus =
        this.statusFilter === 'all' ||
        (this.statusFilter === 'active' && warehouse.isActive) ||
        (this.statusFilter === 'inactive' && !warehouse.isActive);

      return matchesSearch && matchesStatus;
    });
  }

  get activeCount(): number {
    return this.warehouses.filter(w => w.isActive).length;
  }

  get inactiveCount(): number {
    return this.warehouses.filter(w => !w.isActive).length;
  }

  loadWarehouses(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.http.get<Warehouse[]>(this.apiUrl).subscribe({
      next: (data) => {
        this.warehouses = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message ??
          'Depolar yüklenemedi. API servisinin çalıştığını kontrol et.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openCreateForm(): void {
    this.editingId = null;
    this.form = this.emptyForm();
    this.errorMessage = '';
    this.successMessage = '';
    this.showForm = true;
  }

  openEditForm(warehouse: Warehouse): void {
    this.editingId = warehouse.id;
    this.form = {
      code: warehouse.code,
      name: warehouse.name,
      address: warehouse.address ?? ''
    };
    this.errorMessage = '';
    this.successMessage = '';
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.editingId = null;
    this.form = this.emptyForm();
  }

  saveWarehouse(): void {
    if (this.isSaving) return;

    const code = this.form.code.trim();
    const name = this.form.name.trim();

    if (code.length < 2 || name.length < 2) {
      this.errorMessage = 'Depo kodu ve adı en az 2 karakter olmalıdır.';
      return;
    }

    if (code.length > 30 || name.length > 150) {
      this.errorMessage = 'Depo kodu en fazla 30, adı en fazla 150 karakter olabilir.';
      return;
    }

    if ((this.form.address ?? '').trim().length > 300) {
      this.errorMessage = 'Adres en fazla 300 karakter olabilir.';
      return;
    }

    const payload = {
      code,
      name,
      address: this.form.address.trim() || null
    };

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request = this.editingId === null
      ? this.http.post<Warehouse>(this.apiUrl, payload)
      : this.http.put<Warehouse>(`${this.apiUrl}/${this.editingId}`, payload);
    const wasEditing = this.editingId !== null;
    request.subscribe({
      next: () => {
        this.isSaving = false;
        this.closeForm();
        this.successMessage = wasEditing
          ? 'Depo bilgileri güncellendi.'
          : 'Depo başarıyla eklendi.';

        // editingId closeForm içinde sıfırlandığı için mesajı
        // bir sonraki adımda işlem öncesinde belirliyoruz.
        this.loadWarehouses();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message ??
          'İşlem başarısız oldu. Bilgileri kontrol edip tekrar dene.';
        this.isSaving = false;
      }
    });
  }


  toggleStatus(warehouse: Warehouse): void {
    const newStatus = !warehouse.isActive;
    const action = newStatus ? 'aktif' : 'pasif';

    // Aktif etme işleminde doğrudan onay iste.
    if (newStatus) {
      if (!confirm(`"${warehouse.name}" deposunu aktif yapmak istiyor musun?`)) {
        return;
      }

      this.updateWarehouseStatus(warehouse, newStatus, action);
      return;
    }

    // Pasife almadan önce depodaki mevcut stokları kontrol et.
    this.http.get<any[]>('http://localhost:5252/api/stock-balances')
      .subscribe({
        next: (stocks) => {
          const warehouseStocks = stocks.filter(
            stock =>
              stock.warehouseId === warehouse.id &&
              stock.quantity > 0
          );

          const totalQuantity = warehouseStocks.reduce(
            (total, stock) => total + stock.quantity,
            0
          );

          let message =
            `"${warehouse.name}" deposunu pasif yapmak istiyor musun?`;

          if (warehouseStocks.length > 0) {
            message +=
              `\n\nDikkat! Bu depoda ${warehouseStocks.length} farklı stok kaydı` +
              ` ve toplam ${totalQuantity} birim stok bulunuyor.` +
              '\nMevcut stoklar silinmeyecek ancak depo pasifken stok işlemi yapılamayacak.';
          }

          if (!confirm(message)) {
            return;
          }

          this.updateWarehouseStatus(warehouse, false, action);
        },
        error: () => {
          this.errorMessage =
            'Depodaki stoklar kontrol edilemedi. Depo pasife alınmadı.';
        }
      });
  }

  private updateWarehouseStatus(
    warehouse: Warehouse,
    isActive: boolean,
    action: string
  ): void {
    this.errorMessage = '';
    this.successMessage = '';

    this.http.patch<Warehouse>(
      `${this.apiUrl}/${warehouse.id}/status`,
      { isActive }
    ).subscribe({
      next: () => {
        this.successMessage =
          `${warehouse.name} deposu ${action} yapıldı.`;
        this.loadWarehouses();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message ?? 'Depo durumu değiştirilemedi.';
      }
    });
  } 
}
