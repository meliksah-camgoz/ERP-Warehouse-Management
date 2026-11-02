
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <div class="erp-layout">
      <aside class="sidebar">
        <div class="brand">
          <div class="brand-logo">M</div>
          <span>MiniERP</span>
        </div>

        <div class="menu-label">ANA MENÜ</div>

        <nav class="menu">
          <a routerLink="/dashboard" routerLinkActive="active" class="menu-item">
            <span>▦</span> Genel Bakış
          </a>
          <a routerLink="/products" routerLinkActive="active" class="menu-item">
            <span>□</span> Ürünler
          </a>
          <a routerLink="/categories" routerLinkActive="active" class="menu-item">
            <span>▤</span> Kategoriler
          </a>
          <a routerLink="/warehouses" routerLinkActive="active" class="menu-item">
            <span>▣</span> Depolar
          </a>
          <a routerLink="/stock" routerLinkActive="active" class="menu-item">
            <span>⇄</span> Stok Yönetimi
          </a>
          <a routerLink="/assets" routerLinkActive="active" class="menu-item">
            <span>▧</span> Bilgi İşlem Envanteri
          </a>
        </nav>

        <div class="sidebar-bottom">
          <div class="user-avatar">B</div>
          <div class="user-info">
            <strong>bilgiislem</strong>
            <span>Sistem Yöneticisi</span>
          </div>
        </div>
      </aside>

      <main class="main-content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      min-height: 100vh;
      color: #202b3c;
      font-family: Inter, "Segoe UI", Arial, sans-serif;
    }

    * { box-sizing: border-box; }

    .erp-layout {
      display: flex;
      min-height: 100vh;
      background: #f5f7fb;
    }

    .sidebar {
      width: 250px;
      flex-shrink: 0;
      display: flex;
      flex-direction: column;
      padding: 28px 18px 20px;
      background: #172c27;
      color: white;
    }

    .brand {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 0 10px 34px;
      font-size: 21px;
      font-weight: 750;
    }

    .brand-logo {
      display: grid;
      place-items: center;
      width: 38px;
      height: 38px;
      border-radius: 12px;
      background: #d5f3df;
      color: #1b4938;
      font-size: 21px;
      font-weight: 800;
    }

    .menu-label {
      padding: 0 12px 12px;
      color: #8fa79c;
      font-size: 10px;
      font-weight: 750;
      letter-spacing: 1.5px;
    }

    .menu {
      display: grid;
      gap: 7px;
    }

    .menu-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px;
      border-radius: 9px;
      color: #c1d0c9;
      text-decoration: none;
      font-size: 13px;
    }

    .menu-item > span {
      width: 20px;
      text-align: center;
      font-size: 17px;
    }

    .menu-item:hover,
    .menu-item.active {
      background: #29463b;
      color: white;
    }

    .menu-item.active {
      box-shadow: inset 3px 0 #8ee0a9;
    }

    .sidebar-bottom {
      display: flex;
      align-items: center;
      gap: 11px;
      margin-top: auto;
      padding: 20px 5px 0;
      border-top: 1px solid #355046;
    }

    .user-avatar {
      display: grid;
      place-items: center;
      width: 38px;
      height: 38px;
      border-radius: 50%;
      background: #d6eee0;
      color: #214b3b;
      font-weight: 750;
    }

    .user-info { display: grid; gap: 4px; }
    .user-info strong { font-size: 12px; }
    .user-info span { color: #9bb1a6; font-size: 11px; }

    .main-content {
      flex: 1;
      min-width: 0;
      padding: 0 28px;
    }

    @media (max-width: 700px) {
      .sidebar { width: 66px; padding: 20px 9px; }
      .brand { padding: 0 4px 30px; }
      .brand > span, .menu-label, .menu-item:not(.active) {
        display: none;
      }
      .menu-item {
        justify-content: center;
        padding: 13px 8px;
        font-size: 0;
      }
      .menu-item > span { font-size: 19px; }
      .sidebar-bottom { justify-content: center; padding-top: 15px; }
      .user-info { display: none; }
      .main-content { padding: 0 15px; }
    }
  `]
})
export class MainLayout { }
