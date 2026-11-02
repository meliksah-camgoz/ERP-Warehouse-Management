
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface Category {
  id: number;
  name: string;
  description: string | null;
  isActive: boolean;
  createdAtUtc: string;
}

interface CategoryForm {
  name: string;
  description: string;
}

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './categories.html',
  styleUrl: './categories.css'
})
export class Categories implements OnInit {
  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);

  private apiUrl = '/api/categories';

  categories: Category[] = [];
  searchTerm = '';
  statusFilter = 'all';

  isLoading = false;
  isSaving = false;
  showForm = false;
  editingId: number | null = null;

  errorMessage = '';
  successMessage = '';

  form: CategoryForm = {
    name: '',
    description: ''
  };

  ngOnInit(): void {
    this.loadCategories();
  }

  get filteredCategories(): Category[] {
    const term = this.searchTerm.trim().toLocaleLowerCase('tr-TR');

    return this.categories.filter(category => {
      const matchesSearch =
        category.name.toLocaleLowerCase('tr-TR').includes(term) ||
        (category.description ?? '').toLocaleLowerCase('tr-TR').includes(term);

      const matchesStatus =
        this.statusFilter === 'all' ||
        (this.statusFilter === 'active' && category.isActive) ||
        (this.statusFilter === 'inactive' && !category.isActive);

      return matchesSearch && matchesStatus;
    });
  }

  get activeCount(): number {
    return this.categories.filter(c => c.isActive).length;
  }

  get inactiveCount(): number {
    return this.categories.filter(c => !c.isActive).length;
  }

  loadCategories(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.http.get<Category[]>(this.apiUrl).subscribe({
      next: data => {
        this.categories = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.errorMessage =
          error.error?.message ?? 'Kategoriler yÃ¼klenemedi.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openCreateForm(): void {
    this.editingId = null;
    this.form = { name: '', description: '' };
    this.errorMessage = '';
    this.successMessage = '';
    this.showForm = true;
  }

  openEditForm(category: Category): void {
    this.editingId = category.id;
    this.form = {
      name: category.name,
      description: category.description ?? ''
    };
    this.errorMessage = '';
    this.successMessage = '';
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.editingId = null;
    this.form = { name: '', description: '' };
  }

  saveCategory(): void {
    const name = this.form.name.trim();
    const description = this.form.description.trim();

    if (name.length < 2) {
      this.errorMessage = 'Kategori adÄ± en az 2 karakter olmalÄ±dÄ±r.';
      return;
    }

    const payload = {
      name,
      description: description || null
    };

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const wasEditing = this.editingId !== null;

    const request = wasEditing
      ? this.http.put(
        `${this.apiUrl}/${this.editingId}`,
        payload,
        { responseType: 'text' }
      )
      : this.http.post(
        this.apiUrl,
        payload,
        { responseType: 'text' }
      );

    request.subscribe({
      next: () => {
        this.isSaving = false;
        this.closeForm();
        this.successMessage = wasEditing
          ? 'Kategori baÅŸarÄ±yla gÃ¼ncellendi.'
          : 'Kategori baÅŸarÄ±yla oluÅŸturuldu.';
        this.loadCategories();
      },
      error: error => {
        this.isSaving = false;
        this.errorMessage =
          error.error?.message ?? 'Kategori kaydedilemedi.';
        this.cdr.detectChanges();
      }
    });
  }

  toggleStatus(category: Category): void {
    const action = category.isActive ? 'pasif' : 'aktif';

    if (!confirm(`"${category.name}" kategorisini ${action} yapmak istiyor musun?`)) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    if (category.isActive) {
      this.http.patch(
        `${this.apiUrl}/${category.id}/deactivate`,
        {},
        { responseType: 'text' }
      ).subscribe({
        next: () => {
          this.successMessage = 'Kategori pasif yapÄ±ldÄ±.';
          this.loadCategories();
        },
        error: error => {
          this.errorMessage =
            error.error?.message ?? 'Kategori durumu deÄŸiÅŸtirilemedi.';
          this.cdr.detectChanges();
        }
      });

      return;
    }

    this.errorMessage =
      'Kategori yeniden aktifleÅŸtirme iÅŸlemi mevcut API tarafÄ±ndan desteklenmiyor.';
  }
}

