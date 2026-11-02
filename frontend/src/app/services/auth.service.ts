
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);

  // Aynı sunucudaki API'ye göreceli adres
  private apiUrl = '/api/auth';

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`,
      request
    );
  }

  saveToken(token: string): void {
    sessionStorage.setItem('accessToken', token);
  }

  getToken(): string | null {
    return sessionStorage.getItem('accessToken');
  }

  logout(): void {
    sessionStorage.removeItem('accessToken');
  }
}
