
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);

  username = '';
  password = '';
  errorMessage = '';
  isLoading = false;

  login(event: Event): void {
    event.preventDefault();
    this.errorMessage = '';

    if (!this.username.trim() || !this.password) {
      this.errorMessage = 'Kullanıcı adı ve şifre zorunludur.';
      return;
    }

    this.isLoading = true;

    this.authService.login({
      username: this.username.trim(),
      password: this.password
    }).subscribe({
      next: (response) => {
        this.authService.saveToken(response.accessToken);
        this.isLoading = false;
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage =
          error.status === 401
            ? 'Kullanıcı adı veya şifre hatalı.'
            : 'Sunucuya bağlanılamadı. API çalışıyor mu kontrol et.';
      }
    });
  }
}
