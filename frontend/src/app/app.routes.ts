
import { Routes } from '@angular/router';
import { Login } from './login';
import { Dashboard } from './dashboard';
import { Products } from './products/products';
import { MainLayout } from './main-layout';
import { Stock } from './stock/stock';
import { Warehouses } from './warehouses/warehouses';
import { Categories } from './categories/categories';
import { Assets } from './assets/assets';

export const routes: Routes = [
  { path: '', component: Login },
  {
    path: '',
    component: MainLayout,
    children: [
      { path: 'dashboard', component: Dashboard },
      { path: 'products', component: Products },
      { path: 'stock', component: Stock },
      { path: 'warehouses', component: Warehouses },
      { path: 'categories', component: Categories },
      { path: 'assets', component: Assets },
    ]
  },
  { path: '**', redirectTo: '' }
];
