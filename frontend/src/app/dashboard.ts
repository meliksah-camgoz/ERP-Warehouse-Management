import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { DashboardService } from './services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="dashboard-content">
<main class="dashboard-main">
        <header class="topbar">
          <div>
            <span class="breadcrumb">MiniERP / Genel Bakış</span>
            <h1>Genel Bakış</h1>
          </div>

          <div class="topbar-right">
            <span class="status-dot"></span>
            <span class="system-status">Sistem aktif</span>
          </div>
        </header>

        <section class="welcome">
          <div>
            <span class="welcome-tag">YÖNETİM PANELİ</span>
            <h2>Hoş geldiniz 👋</h2>
            <p>İşletmenizin genel durumunu buradan takip edebilirsiniz.</p>
          </div>
          <div class="welcome-decoration">M</div>
        </section>

        <section class="section-heading">
          <div>
            <h2>Genel durum</h2>
            <p>Sistemin temel göstergeleri</p>
          </div>
        </section>

        <section class="stats-grid">
          <article class="stat-card">
            <div class="stat-top">
              <span class="stat-icon purple">□</span>
              <span class="stat-label">ENVANTER</span>
            </div>
            <div class="stat-value">{{ summary.assets }}</div>
            <div class="stat-description">Kayıtlı demirbaşlar</div>
          </article>

          <article class="stat-card">
            <div class="stat-top">
              <span class="stat-icon blue">▤</span>
              <span class="stat-label">ÜRÜNLER</span>
            </div>
            <div class="stat-value">{{ summary.products }}</div>
            <div class="stat-description">Kayıtlı ürün çeşidi</div>
          </article>

          <article class="stat-card">
            <div class="stat-top">
              <span class="stat-icon green">▣</span>
              <span class="stat-label">DEPOLAR</span>
            </div>
            <div class="stat-value">{{ summary.warehouses }}</div>
            <div class="stat-description">Tanımlı depolar</div>
          </article>

          <article class="stat-card">
            <div class="stat-top">
              <span class="stat-icon orange">⇄</span>
              <span class="stat-label">STOK DURUMU</span>
            </div>
            <div class="stat-value">{{ summary.stockBalances }}</div>
            <div class="stat-description">Stok kayıtları</div>
          </article>
        </section>

        <section class="content-grid">
          <article class="content-card">
            <div class="card-heading">
              <div>
                <h3>Hızlı erişim</h3>
                <p>Sık kullanılan işlemler</p>
              </div>
            </div>

            <div class="quick-actions">
              <a href="/assets" class="quick-action">
                <span class="quick-icon purple">▧</span>
                <span>
                  <strong>Envanteri görüntüle</strong>
                  <small>Bilgi işlem demirbaşları</small>
                </span>
                <span class="arrow">→</span>
              </a>

              <a href="/products" class="quick-action">
                <span class="quick-icon blue">□</span>
                <span>
                  <strong>Ürünleri yönet</strong>
                  <small>Ürün kayıtlarını incele</small>
                </span>
                <span class="arrow">→</span>
              </a>

              <a href="/warehouses" class="quick-action">
                <span class="quick-icon green">▣</span>
                <span>
                  <strong>Depoları görüntüle</strong>
                  <small>Depo kayıtlarına ulaş</small>
                </span>
                <span class="arrow">→</span>
              </a>
            </div>
          </article>

          <article class="content-card system-card">
            <div class="card-heading">
              <div>
                <h3>Sistem bilgisi</h3>
                <p>Uygulama durumu</p>
              </div>
              <span class="system-icon">✓</span>
            </div>

            <div class="system-row">
              <span>Uygulama</span>
              <strong>MiniERP</strong>
            </div>
            <div class="system-row">
              <span>Ortam</span>
              <strong>Development</strong>
            </div>
            <div class="system-row">
              <span>Kimlik doğrulama</span>
              <strong class="connected">JWT etkin</strong>
            </div>
            <div class="system-row">
              <span>Veritabanı</span>
              <strong>SQL Server</strong>
            </div>
          </article>
        </section>

        <footer class="dashboard-footer">
          <span>© 2026 MiniERP</span>
          <span>Kurumsal Kaynak Planlama</span>
        </footer>
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

    .dashboard-content {
      width: 100%;
      min-width: 0;
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
      color: #fff;
    }

    .brand {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 0 10px 34px;
      font-size: 21px;
      font-weight: 750;
      letter-spacing: -.5px;
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
      transition: background .2s, color .2s;
    }

    .menu-item > span {
      width: 20px;
      text-align: center;
      font-size: 17px;
    }

    .menu-item:hover,
    .menu-item.active {
      background: #29463b;
      color: #fff;
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

    .dashboard-main {
      width: 100%;
      max-width: 1800px;
      margin: 0 auto;
      padding: 0;
      min-width: 0;
    }

    .topbar {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 20px;
      padding: 29px 0 25px;
      border-bottom: 1px solid #e6eaf0;
    }

    .breadcrumb {
      color: #8a95a6;
      font-size: 11px;
    }

    .topbar h1 {
      margin: 8px 0 0;
      font-size: 25px;
      letter-spacing: -.7px;
    }

    .topbar-right {
      display: flex;
      align-items: center;
      gap: 8px;
      color: #647386;
      font-size: 12px;
    }

    .status-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: #31b779;
      box-shadow: 0 0 0 4px #e1f6eb;
    }

    .welcome {
      position: relative;
      display: flex;
      align-items: center;
      justify-content: space-between;
      overflow: hidden;
      min-height: 175px;
      margin-top: 28px;
      padding: 30px 34px;
      border-radius: 15px;
      background: linear-gradient(110deg, #e4f4e9, #eff8f1 68%, #d5eddf);
    }

    .welcome-tag {
      color: #39805d;
      font-size: 10px;
      font-weight: 800;
      letter-spacing: 1.5px;
    }

    .welcome h2 {
      margin: 11px 0 8px;
      font-size: 25px;
      letter-spacing: -.7px;
    }

    .welcome p {
      margin: 0;
      color: #667a70;
      font-size: 13px;
    }

    .welcome-decoration {
      position: absolute;
      right: 45px;
      top: -40px;
      color: #c9e8d4;
      font-size: 210px;
      line-height: 1;
      font-weight: 850;
      user-select: none;
    }

    .section-heading {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin: 30px 0 16px;
    }

    .section-heading h2,
    .card-heading h3 {
      margin: 0;
      font-size: 16px;
      letter-spacing: -.3px;
    }

    .section-heading p,
    .card-heading p {
      margin: 6px 0 0;
      color: #8a95a6;
      font-size: 12px;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(4, minmax(0, 1fr));
      gap: 17px;
    }

    .stat-card,
    .content-card {
      border: 1px solid #e8edf3;
      border-radius: 13px;
      background: #fff;
      box-shadow: 0 3px 12px rgb(27 45 70 / 3%);
    }

    .stat-card { padding: 20px; }

    .stat-top {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 8px;
    }

    .stat-icon,
    .quick-icon {
      display: grid;
      place-items: center;
      flex-shrink: 0;
      width: 38px;
      height: 38px;
      border-radius: 10px;
      font-size: 19px;
    }

    .purple { background: #f0eaff; color: #7857ce; }
    .blue { background: #e6f1ff; color: #3978ca; }
    .green { background: #e1f6e9; color: #26945b; }
    .orange { background: #fff0df; color: #d78b32; }

    .stat-label {
      color: #9aa4b3;
      font-size: 9px;
      font-weight: 800;
      letter-spacing: 1px;
    }

    .stat-value {
      margin-top: 21px;
      font-size: 30px;
      font-weight: 750;
      letter-spacing: -1px;
    }

    .stat-description {
      margin-top: 6px;
      color: #8792a2;
      font-size: 11px;
    }

    .content-grid {
      display: grid;
      grid-template-columns: minmax(0, 1.3fr) minmax(280px, 1fr);
      gap: 18px;
      margin-top: 22px;
    }

    .content-card { padding: 23px; }
    .card-heading {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 18px;
    }

    .quick-actions { display: grid; }

    .quick-action {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 14px 0;
      border-top: 1px solid #edf0f4;
      color: inherit;
      text-decoration: none;
    }

    .quick-action > span:nth-child(2) {
      display: grid;
      gap: 5px;
      min-width: 0;
    }

    .quick-action strong { font-size: 12px; }
    .quick-action small { color: #8994a4; font-size: 11px; }
    .quick-action .arrow {
      margin-left: auto;
      color: #8793a2;
      font-size: 18px;
    }

    .quick-action:hover strong { color: #2d8657; }

    .system-icon {
      display: grid;
      place-items: center;
      width: 31px;
      height: 31px;
      border-radius: 50%;
      background: #e2f7eb;
      color: #26945b;
      font-weight: 800;
    }

    .system-row {
      display: flex;
      justify-content: space-between;
      gap: 15px;
      padding: 14px 0;
      border-top: 1px solid #edf0f4;
      font-size: 12px;
    }

    .system-row > span { color: #8490a0; }
    .system-row strong { font-weight: 650; }
    .system-row .connected { color: #26945b; }

    .dashboard-footer {
      display: flex;
      justify-content: space-between;
      gap: 15px;
      padding: 28px 0 22px;
      color: #9aa4b3;
      font-size: 10px;
    }

    @media (max-width: 1100px) {
      .dashboard-main { padding: 0; }
      .stats-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
      .content-grid { grid-template-columns: 1fr; }
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
      .dashboard-main { width: 100%; padding: 0; }
      .topbar { padding: 22px 0; }
      .topbar h1 { font-size: 21px; }
      .system-status { display: none; }
      .welcome { min-height: 155px; padding: 23px 20px; }
      .welcome h2 { font-size: 21px; }
      .welcome p { max-width: 240px; line-height: 1.6; }
      .welcome-decoration { right: -10px; font-size: 145px; }
      .stats-grid { gap: 10px; }
      .stat-card { padding: 14px; }
      .stat-label { font-size: 8px; }
      .stat-value { font-size: 25px; }
      .content-card { padding: 17px; }
      .dashboard-footer { flex-direction: column; }
    }
  `]
})
export class Dashboard implements OnInit {

  private dashboardService = inject(DashboardService);
  private cdr = inject(ChangeDetectorRef);

  summary = {
    assets: 0,
    products: 0,
    warehouses: 0,
    stockBalances: 0
  };

  loading = true;
  loadError = '';

  ngOnInit(): void {
    this.dashboardService.getSummary().subscribe({
      next: (data) => {
        this.summary = { ...data };
        this.cdr.detectChanges();
        console.log('Kartlara aktarılan veriler:', this.summary);
      },
      error: (error) => {
        console.error('Dashboard API hatası:', error);
        this.loadError = 'Dashboard verileri yüklenemedi.';
        this.loading = false;
      }
    });
  }

}
