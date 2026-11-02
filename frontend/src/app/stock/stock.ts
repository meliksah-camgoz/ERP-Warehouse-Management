
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';

interface StockBalance {
  id: number;
  productId: number;
  productName: string;
  sku: string;
  warehouseId: number;
  warehouseName: string;
  quantity: number;
  updatedAtUtc: string;
}

interface StockMovement {
  id: number;
  productId: number;
  productName: string;
  sku: string;
  warehouseId: number;
  warehouseName: string;
  movementType: string;
  quantity: number;
  createdAtUtc: string;
  description?: string | null;
}


interface Product {
  id: number;
  name: string;
  sku: string;
  minStockLevel: number;
}

interface Warehouse {
  id: number;
  name: string;
  isActive: boolean;
}

type StockOperation = 'entry' | 'exit';

@Component({
  selector: 'app-stock',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './stock.html',
  styleUrl: './stock.css'
})
export class Stock implements OnInit {
  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);

  private apiUrl = 'http://localhost:5252/api';

  stocks: StockBalance[] = [];
  movements: StockMovement[] = [];
  isLoadingMovements = false;
  movementError = '';
  products: Product[] = [];
  warehouses: Warehouse[] = [];
  get lowStockItems(): StockBalance[] {
    return this.stocks.filter(stock => {
      const product = this.products.find(
        item => item.id === stock.productId
      );

      if (!product) {
        return false;
      }

      return stock.quantity <= product.minStockLevel;
    });
  }

  searchTerm = '';
  isLoading = false;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  showForm = false;
  operation: StockOperation = 'entry';

  form = {
    productId: 0,
    warehouseId: 0,
    quantity: 1
  };

  get filteredStocks(): StockBalance[] {
    const term = this.searchTerm.trim().toLocaleLowerCase('tr-TR');

    return this.stocks.filter(stock =>
      stock.productName.toLocaleLowerCase('tr-TR').includes(term) ||
      stock.sku.toLocaleLowerCase('tr-TR').includes(term) ||
      stock.warehouseName.toLocaleLowerCase('tr-TR').includes(term)
    );
  }

  get totalQuantity(): number {
    return this.stocks.reduce((total, stock) => total + stock.quantity, 0);
  }

  ngOnInit(): void {
    this.loadStocks();
    this.loadProducts();
    this.loadWarehouses();
    this.loadMovements();
  }

  loadStocks(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.http.get<StockBalance[]>(`${this.apiUrl}/stock-balances`)
      .subscribe({
        next: data => {
          this.stocks = data;
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: err => {
          this.errorMessage =
            err.error?.message ?? 'Stok listesi yüklenemedi.';
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
  }

  loadProducts(): void {
    this.http.get<Product[]>(`${this.apiUrl}/products`)
      .subscribe({
        next: data => {
          this.products = data;
          this.cdr.detectChanges();
        },
        error: () => {
          this.errorMessage = 'Ürün listesi yüklenemedi.';
          this.cdr.detectChanges();
        }
      });
  }

  loadWarehouses(): void {
    this.http.get<Warehouse[]>(`${this.apiUrl}/warehouses`)
      .subscribe({
        next: data => {
          this.warehouses = data.filter(w => w.isActive);
          this.cdr.detectChanges();
        },
        error: () => {
          this.errorMessage = 'Depo listesi yüklenemedi.';
          this.cdr.detectChanges();
        }
      });
  }


  loadMovements(): void {
    this.isLoadingMovements = true;
    this.movementError = '';

    this.http.get<StockMovement[]>(`${this.apiUrl}/stock-movements`)
      .subscribe({
        next: data => {
          this.movements = data;
          this.isLoadingMovements = false;
          this.cdr.detectChanges();
        },
        error: err => {
          this.movementError =
            err.error?.message ?? 'Stok hareketleri yüklenemedi.';
          this.isLoadingMovements = false;
          this.cdr.detectChanges();
        }
      });
  }

  openForm(operation: StockOperation): void {
    this.operation = operation;
    this.form = {
      productId: this.products[0]?.id ?? 0,
      warehouseId: this.warehouses[0]?.id ?? 0,
      quantity: 1
    };
    this.errorMessage = '';
    this.successMessage = '';
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
  }

  saveStock(): void {
    if (
      this.form.productId <= 0 ||
      this.form.warehouseId <= 0 ||
      !Number.isFinite(this.form.quantity) ||
      this.form.quantity <= 0
    ) {
      this.errorMessage = 'Ürün, depo ve geçerli bir miktar seçmelisin.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const endpoint = this.operation === 'entry' ? 'entry' : 'exit';

    this.http.post(
      `${this.apiUrl}/stock-balances/${endpoint}`,
      {
        productId: this.form.productId,
        warehouseId: this.form.warehouseId,
        quantity: this.form.quantity
      }
    ).subscribe({
      next: () => {
        this.isSaving = false;
        this.showForm = false;
        this.successMessage =
          this.operation === 'entry'
            ? 'Stok girişi başarıyla kaydedildi.'
            : 'Stok çıkışı başarıyla kaydedildi.';

        this.loadStocks();
        this.loadMovements();
        this.cdr.detectChanges();
      },
      error: err => {
        this.isSaving = false;
        this.errorMessage =
          err.error?.message ??
          'Stok işlemi sırasında bir hata oluştu.';
        this.cdr.detectChanges();
      }
    });
  }
}
