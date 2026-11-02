
import {
  Component,
  inject,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface Product {
  id: number;
  categoryId: number;
  categoryName?: string;
  sku: string;
  barcode?: string;
  name: string;
  unit: string;
  purchasePrice: number;
  salePrice: number;
  vatRate: number;
  minStockLevel: number;
  isActive: boolean;
  createdAtUtc?: string;
  description?: string;
  code?: string;
}

interface Category {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
}

interface ProductForm {
  categoryId: number;
  sku: string;
  barcode: string;
  name: string;
  unit: string;
  purchasePrice: number;
  salePrice: number;
  vatRate: number;
  minStockLevel: number;
}

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page">
      <header class="topbar">
        <div>
          <span class="breadcrumb">MiniERP / Ürünler</span>
          <h1>Ürünler</h1>
          <p>Ürün kayıtlarını görüntüleyin ve yönetin.</p>
        </div>

        <div class="top-actions">
          <button
            class="secondary-button"
            (click)="loadProducts()"
            [disabled]="loading"
          >
            ↻ Yenile
          </button>

          <button class="primary-button" (click)="openCreateForm()">
            + Yeni Ürün
          </button>
        </div>
      </header>

      @if (errorMessage) {
        <div class="alert error">{{ errorMessage }}</div>
      }

      @if (successMessage) {
        <div class="alert success">{{ successMessage }}</div>
      }

      <section class="summary">
        <div class="summary-card">
          <span>Toplam ürün</span>
          <strong>{{ products.length }}</strong>
        </div>

        <div class="summary-card">
          <span>Aktif ürün</span>
          <strong>{{ activeCount }}</strong>
        </div>
      </section>

      @if (formVisible) {
        <section class="form-card">
          <div class="form-heading">
            <div>
              <h2>{{ editingId === null ? 'Yeni Ürün Ekle' : 'Ürünü Düzenle' }}</h2>
              <p>Ürün bilgilerini eksiksiz doldurun.</p>
            </div>

            <button
              class="close-button"
              type="button"
              (click)="closeForm()"
              [disabled]="saving"
              aria-label="Formu kapat"
            >
              ✕
            </button>
          </div>

          @if (categories.length === 0) {
            <div class="alert error">
              Kategori bulunamadı. Önce bir kategori oluşturmalısınız.
            </div>
          }

          <form (submit)="saveProduct($event)">
            <div class="form-grid">
              <label>
                Ürün adı *
                <input
                  name="name"
                  required
                  minlength="2"
                  maxlength="200"
                  [(value)]="form.name"
                  (input)="setText('name', $event)"
                  placeholder="Örn. Logitech Mouse"
                />
              </label>

              <label>
                Ürün kodu (SKU) *
                <input
                  name="sku"
                  required
                  minlength="2"
                  maxlength="50"
                  [(value)]="form.sku"
                  (input)="setText('sku', $event)"
                  placeholder="Örn. MS-001"
                />
              </label>

              <label>
                Kategori *
                <select
                  name="categoryId"
                  required
                  [value]="form.categoryId"
                  (change)="setCategory($event)"
                >
                  <option [value]="0" disabled>Kategori seçin</option>
                  @for (category of categories; track category.id) {
                    @if (category.isActive) {
                      <option [value]="category.id">
                        {{ category.name }}
                      </option>
                    }
                  }
                </select>
              </label>

              <label>
                Barkod
                <input
                  name="barcode"
                  maxlength="100"
                  [value]="form.barcode"
                  (input)="setText('barcode', $event)"
                  placeholder="Barkod (isteğe bağlı)"
                />
              </label>

              <label>
                Birim *
                <select
                  name="unit"
                  required
                  [value]="form.unit"
                  (change)="setUnit($event)"
                >
                  <option value="Adet">Adet</option>
                  <option value="Kutu">Kutu</option>
                  <option value="Paket">Paket</option>
                  <option value="Kg">Kg</option>
                  <option value="Litre">Litre</option>
                  <option value="Metre">Metre</option>
                </select>
              </label>

              <label>
                Alış fiyatı *
                <input
                  name="purchasePrice"
                  type="number"
                  required
                  min="0"
                  step="0.01"
                  [value]="form.purchasePrice"
                  (input)="setNumber('purchasePrice', $event)"
                />
              </label>

              <label>
                Satış fiyatı *
                <input
                  name="salePrice"
                  type="number"
                  required
                  min="0"
                  step="0.01"
                  [value]="form.salePrice"
                  (input)="setNumber('salePrice', $event)"
                />
              </label>

              <label>
                KDV oranı (%) *
                <input
                  name="vatRate"
                  type="number"
                  required
                  min="0"
                  max="100"
                  step="0.01"
                  [value]="form.vatRate"
                  (input)="setNumber('vatRate', $event)"
                />
              </label>

              <label>
                Minimum stok seviyesi *
                <input
                  name="minStockLevel"
                  type="number"
                  required
                  min="0"
                  step="0.001"
                  [value]="form.minStockLevel"
                  (input)="setNumber('minStockLevel', $event)"
                />
              </label>
            </div>

            <div class="form-actions">
              <button
                type="button"
                class="secondary-button"
                (click)="closeForm()"
                [disabled]="saving"
              >
                Vazgeç
              </button>

              <button
                type="submit"
                class="primary-button"
                [disabled]="saving || categories.length === 0"
              >
                {{
                  saving
                    ? 'Kaydediliyor...'
                    : editingId === null
                      ? 'Ürünü Kaydet'
                      : 'Değişiklikleri Kaydet'
                }}
              </button>
            </div>
          </form>
        </section>
      }

      <section class="table-card">
        <div class="table-header">
          <div>
            <h2>Ürün listesi</h2>
            <p>Kayıtlı ürünleriniz</p>
          </div>

          <input
            class="search-input"
            type="text"
            placeholder="Ürün ara..."
            [value]="searchText"
            (input)="onSearch($event)"
          />
        </div>

        @if (loading && products.length === 0) {
          <div class="message">Ürünler yükleniyor...</div>
        } @else if (filteredProducts.length === 0) {
          <div class="message">Gösterilecek ürün bulunamadı.</div>
        } @else {
          <div class="table-scroll">
            <table>
              <thead>
                <tr>
                  <th>Ürün</th>
                  <th>Kod</th>
                  <th>Kategori</th>
                  <th>Birim</th>
                  <th>Satış fiyatı</th>
                  <th>Durum</th>
                  <th>İşlemler</th>
                </tr>
              </thead>

              <tbody>
                @for (product of filteredProducts; track product.id) {
                  <tr>
                    <td>
                      <strong>{{ product.name || 'İsimsiz ürün' }}</strong>
                      @if (product.barcode) {
                        <small>Barkod: {{ product.barcode }}</small>
                      }
                    </td>

                    <td>{{ product.sku || product.code || '—' }}</td>
                    <td>{{ product.categoryName || '—' }}</td>
                    <td>{{ product.unit || 'Adet' }}</td>
                    <td>{{ product.salePrice | number:'1.2-2' }} ₺</td>

                    <td>
                      <span
                        class="status"
                        [class.inactive]="product.isActive === false"
                      >
                        {{ product.isActive === false ? 'Pasif' : 'Aktif' }}
                      </span>
                    </td>

                    <td>
                      <div class="row-actions">
                        <button
                          class="edit-button"
                          (click)="openEditForm(product)"
                          [disabled]="saving || deletingId !== null"
                        >
                          Düzenle
                        </button>

                        <button
                         class="status-toggle-button"
                         [class.activate-button]="product.isActive === false"
                         [class.deactivate-button]="product.isActive !== false"
                         (click)="toggleProductStatus(product)"
                         [disabled]="saving || deletingId !== null || statusChangingId !== null"
                        >
                          {{
                           statusChangingId === product.id
                             ? 'İşleniyor...'
                             : product.isActive === false
                               ? 'Aktif Et'
                               : 'Pasife Al'
                          }}
                        </button>

                        <button
                          class="delete-button"
                          (click)="deleteProduct(product)"
                          [disabled]="saving || deletingId !== null"
                        >
                          {{
                            deletingId === product.id
                              ? 'Siliniyor...'
                              : 'Sil'
                          }}
                        </button>
                      </div>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </section>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      min-height: 100vh;
      color: #202b3c;
      font-family: Inter, "Segoe UI", Arial, sans-serif;
    }

    * {
      box-sizing: border-box;
    }

    .page {
      min-height: 100vh;
      padding: 0 38px 30px;
      background: #f5f7fb;
    }

    .topbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 20px;
      padding: 29px 0 25px;
      border-bottom: 1px solid #e6eaf0;
    }

    .breadcrumb {
      color: #8a95a6;
      font-size: 11px;
    }

    h1 {
      margin: 8px 0;
      font-size: 25px;
    }

    .topbar p,
    .table-header p,
    .form-heading p {
      margin: 0;
      color: #8a95a6;
      font-size: 12px;
    }

    .top-actions,
    .form-actions,
    .row-actions {
      display: flex;
      align-items: center;
      gap: 9px;
    }

    button {
      font: inherit;
      cursor: pointer;
    }

    button:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .primary-button,
    .secondary-button {
      border-radius: 9px;
      padding: 11px 16px;
      font-size: 12px;
      font-weight: 600;
      white-space: nowrap;
    }

    .primary-button {
      border: 1px solid #246b4b;
      background: #246b4b;
      color: white;
    }

    .primary-button:hover:not(:disabled) {
      background: #1d583d;
    }

    .secondary-button {
      border: 1px solid #dce3eb;
      background: white;
      color: #344054;
    }

    .summary {
      display: grid;
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 16px;
      margin: 25px 0;
    }

    .summary-card,
    .table-card,
    .form-card {
      background: white;
      border: 1px solid #e8edf3;
      border-radius: 13px;
      box-shadow: 0 3px 12px rgb(27 45 70 / 3%);
    }

    .summary-card {
      display: grid;
      gap: 12px;
      padding: 20px;
    }

    .summary-card span {
      color: #8490a0;
      font-size: 12px;
    }

    .summary-card strong {
      font-size: 28px;
    }

    .table-card {
      overflow: hidden;
    }

    .table-header,
    .form-heading {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 15px;
      padding: 22px;
      border-bottom: 1px solid #edf0f4;
    }

    .table-header h2,
    .form-heading h2 {
      margin: 0 0 7px;
      font-size: 16px;
    }

    .search-input {
      width: 240px;
      max-width: 100%;
    }

    input,
    select {
      width: 100%;
      min-width: 0;
      padding: 10px 12px;
      border: 1px solid #dce3eb;
      border-radius: 8px;
      background: white;
      color: #202b3c;
      outline: none;
      font: inherit;
      font-size: 12px;
    }

    input:focus,
    select:focus {
      border-color: #4b9870;
      box-shadow: 0 0 0 3px rgb(75 152 112 / 10%);
    }

    .form-card {
      margin-bottom: 25px;
      overflow: hidden;
    }

    .form-grid {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: 20px;
      padding: 22px;
    }

    .form-grid label {
      display: grid;
      gap: 8px;
      color: #475467;
      font-size: 12px;
      font-weight: 600;
    }

    .form-actions {
      justify-content: flex-end;
      padding: 0 22px 22px;
    }

    .close-button {
      width: 34px;
      height: 34px;
      border: 1px solid #e8edf3;
      border-radius: 8px;
      background: white;
      color: #667085;
    }

    .table-scroll {
      overflow-x: auto;
    }

    table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
      font-size: 13px;
    }

    th,
    td {
      padding: 16px 18px;
      border-bottom: 1px solid #edf0f4;
    }

    th {
      background: #fafbfd;
      color: #8490a0;
      font-size: 10px;
      letter-spacing: .5px;
      text-transform: uppercase;
      white-space: nowrap;
    }

    td strong,
    td small {
      display: block;
    }

    td small {
      margin-top: 5px;
      color: #8490a0;
      font-size: 11px;
    }

    .status {
      display: inline-block;
      padding: 5px 9px;
      border-radius: 20px;
      background: #e2f7eb;
      color: #258451;
      font-size: 11px;
      font-weight: 650;
    }

    .status.inactive {
      background: #fce8e8;
      color: #b54747;
    }

    .edit-button,
    .delete-button {
      border-radius: 7px;
      padding: 7px 10px;
      font-size: 11px;
      font-weight: 600;
      white-space: nowrap;
    }

    .edit-button {
      border: 1px solid #dce3eb;
      background: white;
      color: #344054;
    }

    .edit-button:hover:not(:disabled) {
      background: #f5f7fb;
    }

    .delete-button {
      border: 1px solid #f1caca;
      background: #fff5f5;
      color: #c03939;
    }

    .delete-button:hover:not(:disabled) {
      background: #fee2e2;
    }

    .status-toggle-button {
      border-radius: 7px;
      padding: 7px 10px;
      font-size: 11px;
      font-weight: 600;
      white-space: nowrap;
    }

    .activate-button {
      border: 1px solid #b7e4c7;
      background: #ecfdf3;
      color: #16834a;
    }

    .activate-button:hover:not(:disabled) {
      background: #dcfce7;
    }

    .deactivate-button {
      border: 1px solid #f5d6a5;
      background: #fff8eb;
      color: #a16207;
    }

    .deactivate-button:hover:not(:disabled) {
      background: #fef3c7;
    }

    .message {
      padding: 35px 20px;
      color: #8490a0;
      text-align: center;
      font-size: 13px;
    }

    .alert {
      margin-top: 18px;
      padding: 13px 16px;
      border-radius: 9px;
      font-size: 13px;
    }

    .error {
      background: #fce8e8;
      color: #b42318;
    }

    .success {
      background: #e2f7eb;
      color: #258451;
    }

    @media (max-width: 950px) {
      .form-grid {
        grid-template-columns: repeat(2, minmax(0, 1fr));
      }
    }

    @media (max-width: 700px) {
      .page {
        padding: 0 15px 25px;
      }

      .topbar {
        align-items: flex-start;
        flex-direction: column;
      }

      .top-actions {
        width: 100%;
      }

      .summary {
        gap: 10px;
      }

      .table-header {
        align-items: stretch;
        flex-direction: column;
      }

      .search-input {
        width: 100%;
      }

      .form-grid {
        grid-template-columns: 1fr;
        gap: 16px;
      }

      th,
      td {
        padding: 13px;
      }
    }
  `]
})
export class Products implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5252/api';
  private cdr = inject(ChangeDetectorRef);

  products: Product[] = [];
  categories: Category[] = [];

  loading = true;
  saving = false;
  deletingId: number | null = null;
  statusChangingId: number | null = null;

  errorMessage = '';
  successMessage = '';
  searchText = '';

  formVisible = false;
  editingId: number | null = null;

  form: ProductForm = this.emptyForm();

  private emptyForm(): ProductForm {
    return {
      categoryId: 0,
      sku: '',
      barcode: '',
      name: '',
      unit: 'Adet',
      purchasePrice: 0,
      salePrice: 0,
      vatRate: 20,
      minStockLevel: 0
    };
  }

  get activeCount(): number {
    return this.products.filter(
      product => product.isActive !== false
    ).length;
  }

  get filteredProducts(): Product[] {
    const query = this.searchText.trim().toLocaleLowerCase('tr-TR');

    if (!query) {
      return this.products;
    }

    return this.products.filter(product =>
      [
        product.name,
        product.sku,
        product.code,
        product.barcode,
        product.categoryName
      ].some(value =>
        value?.toLocaleLowerCase('tr-TR').includes(query)
      )
    );
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.errorMessage = '';

    this.http.get<Product[]>(`${this.apiUrl}/products`).subscribe({
      next: data => {
        this.products = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: error => {
        console.error('Ürünler yüklenemedi:', error);
        this.errorMessage =
          error.error?.message ??
          `Ürünler yüklenemedi (HTTP ${error.status}).`;
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadCategories(): void {
    this.http.get<Category[]>(`${this.apiUrl}/categories`).subscribe({
      next: data => {
        this.categories = data;
        this.cdr.detectChanges();
      },
      error: error => {
        console.error('Kategoriler yüklenemedi:', error);
        this.errorMessage =
          `Kategoriler yüklenemedi (HTTP ${error.status}).`;
        this.cdr.detectChanges();
      }
    });
  }

  onSearch(event: Event): void {
    this.searchText =
      (event.target as HTMLInputElement).value;
  }

  setText(
    field: 'name' | 'sku' | 'barcode',
    event: Event
  ): void {
    this.form[field] =
      (event.target as HTMLInputElement).value;
  }

  setNumber(
    field: 'purchasePrice' | 'salePrice' | 'vatRate' | 'minStockLevel',
    event: Event
  ): void {
    const value = (event.target as HTMLInputElement).value;
    this.form[field] = value === '' ? 0 : Number(value);
  }

  setCategory(event: Event): void {
    this.form.categoryId =
      Number((event.target as HTMLSelectElement).value);
  }

  setUnit(event: Event): void {
    this.form.unit =
      (event.target as HTMLSelectElement).value;
  }

  openCreateForm(): void {
    this.clearMessages();
    this.editingId = null;
    this.form = this.emptyForm();

    const firstActiveCategory =
      this.categories.find(category => category.isActive);

    if (firstActiveCategory) {
      this.form.categoryId = firstActiveCategory.id;
    }

    this.formVisible = true;
  }

  openEditForm(product: Product): void {
    this.clearMessages();
    this.editingId = product.id;

    this.form = {
      categoryId: product.categoryId,
      sku: product.sku || product.code || '',
      barcode: product.barcode || '',
      name: product.name || '',
      unit: product.unit || 'Adet',
      purchasePrice: product.purchasePrice ?? 0,
      salePrice: product.salePrice ?? 0,
      vatRate: product.vatRate ?? 0,
      minStockLevel: product.minStockLevel ?? 0
    };

    this.formVisible = true;
  }

  closeForm(): void {
    if (this.saving) {
      return;
    }

    this.formVisible = false;
    this.editingId = null;
    this.form = this.emptyForm();
  }

  saveProduct(event: Event): void {
    event.preventDefault();

    if (this.saving) {
      return;
    }

    if (!this.form.categoryId) {
      this.errorMessage = 'Lütfen bir kategori seçin.';
      return;
    }

    if (this.form.name.trim().length < 2) {
      this.errorMessage = 'Ürün adı en az 2 karakter olmalıdır.';
      return;
    }

    if (this.form.sku.trim().length < 2) {
      this.errorMessage = 'Ürün kodu en az 2 karakter olmalıdır.';
      return;
    }

    if (
      this.form.purchasePrice < 0 ||
      this.form.salePrice < 0 ||
      this.form.minStockLevel < 0 ||
      this.form.vatRate < 0 ||
      this.form.vatRate > 100
    ) {
      this.errorMessage = 'Lütfen sayısal alanları kontrol edin.';
      return;
    }

    const payload: ProductForm = {
      ...this.form,
      categoryId: Number(this.form.categoryId),
      sku: this.form.sku.trim(),
      barcode: this.form.barcode.trim(),
      name: this.form.name.trim(),
      unit: this.form.unit.trim()
    };

    this.saving = true;
    this.clearMessages();

    const request = this.editingId === null
      ? this.http.post<Product>(
        `${this.apiUrl}/products`,
        payload
      )
      : this.http.put(
        `${this.apiUrl}/products/${this.editingId}`,
        payload
      );

    request.subscribe({
      next: () => {
        const wasEditing = this.editingId !== null;

        this.saving = false;
        this.formVisible = false;
        this.editingId = null;
        this.form = this.emptyForm();

        this.successMessage = wasEditing
          ? 'Ürün başarıyla güncellendi.'
          : 'Ürün başarıyla eklendi.';

        this.loadProducts();
        this.cdr.detectChanges();
      },
      error: error => {
        console.error('Ürün kaydedilemedi:', error);

        this.errorMessage =
          error.error?.message ??
          `Ürün kaydedilemedi (HTTP ${error.status}).`;

        this.saving = false;
        this.cdr.detectChanges();
      }
    });
  }

  toggleProductStatus(product: Product): void {
    if (
      this.statusChangingId !== null ||
      this.deletingId !== null ||
      this.saving
    ) {
      return;
    }

    const newStatus = !product.isActive;
    const action = newStatus ? 'aktifleştirmek' : 'pasife almak';

    const confirmed = window.confirm(
      `"${product.name}" ürününü ${action} istiyor musunuz?`
    );

    if (!confirmed) {
      return;
    }

    this.statusChangingId = product.id;
    this.clearMessages();

    this.http.patch(
      `${this.apiUrl}/products/${product.id}/status`,
      newStatus
    ).subscribe({
      next: () => {
        product.isActive = newStatus;
        this.statusChangingId = null;

        this.successMessage = newStatus
          ? `"${product.name}" yeniden aktifleştirildi.`
          : `"${product.name}" pasife alındı. Stok girişi engellendi.`;

        this.cdr.detectChanges();
      },
      error: error => {
        console.error('Ürün durumu değiştirilemedi:', error);

        this.errorMessage =
          error.error?.message ??
          `Ürün durumu değiştirilemedi (HTTP ${error.status}).`;

        this.statusChangingId = null;
        this.cdr.detectChanges();
      }
    });
  }

  deleteProduct(product: Product): void {
    if (this.deletingId !== null || this.saving) {
      return;
    }

    const confirmed = window.confirm(
      `"${product.name}" ürününü kalıcı olarak silmek istiyor musunuz?\n\n` +
      'Bu işlem geri alınamaz. Ürüne bağlı stok bakiyeleri ve stok hareketleri de silinir.'
    );

    if (!confirmed) {
      return;
    }

    this.deletingId = product.id;
    this.clearMessages();

    this.http.delete(`${this.apiUrl}/products/${product.id}`).subscribe({
      next: () => {
        this.products = this.products.filter(
          item => item.id !== product.id
        );

        this.deletingId = null;
        this.successMessage =
          `"${product.name}" ürünü kalıcı olarak silindi.`;

        this.cdr.detectChanges();
      },
      error: error => {
        console.error('Ürün silinemedi:', error);

        this.errorMessage =
          error.error?.message ??
          `Ürün silinemedi (HTTP ${error.status}).`;

        this.deletingId = null;
        this.cdr.detectChanges();
      }
    });
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}
