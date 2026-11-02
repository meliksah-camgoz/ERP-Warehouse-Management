import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, map, retry, timer } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5252/api';

  getSummary() {
    return forkJoin({
      assets: this.http.get<unknown[]>(`${this.apiUrl}/assets`),
      products: this.http.get<unknown[]>(`${this.apiUrl}/products`),
      warehouses: this.http.get<unknown[]>(`${this.apiUrl}/warehouses`),
      stockBalances: this.http.get<unknown[]>(`${this.apiUrl}/stock-balances`)
    }).pipe(
      map(data => ({
        assets: data.assets.length,
        products: data.products.length,
        warehouses: data.warehouses.length,
        stockBalances: data.stockBalances.length
      }))
    );
    retry({ count: 3, delay: () => timer(1000) })
  }
}
