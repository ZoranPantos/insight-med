import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { ToastService } from './toast.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private toastService = inject(ToastService);

  private apiUrl = '/api'; 
  private tokenKey = 'insight_med_token';
  private logoutTimerId: ReturnType<typeof setTimeout> | null = null;

  constructor() {
    if (this.getToken() && this.isTokenExpired()) {
      localStorage.removeItem(this.tokenKey);
    } else {
      this.scheduleAutoLogout();
    }
  }

  login(payload: any) {
    return this.http.post<{ token: string }>(`${this.apiUrl}/Auth/login`, payload)
      .pipe(
        tap(response => {
          localStorage.setItem(this.tokenKey, response.token);
          this.scheduleAutoLogout();
        })
      );
  }

  logout() {
    this.clearAutoLogout();
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }

  register(payload: any) {
    return this.http.post(`${this.apiUrl}/Auth/register`, payload);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken() && !this.isTokenExpired();
  }

  getUserIdFromToken(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payloadBase64 = token.split('.')[1];
      const payloadJson = atob(payloadBase64);
      const payload = JSON.parse(payloadJson);

      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] 
             || payload.nameid 
             || payload.sub 
             || null;
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }

  isTokenExpired(): boolean {
    const expiryDate = this.getTokenExpiryDate();
    if (!expiryDate) return true;
    return expiryDate.getTime() <= Date.now();
  }

  handleSessionExpired() {
    if (!this.getToken()) return;

    this.clearAutoLogout();
    localStorage.removeItem(this.tokenKey);

    const onAuthPage = this.router.url.startsWith('/login') || this.router.url.startsWith('/register');

    if (!onAuthPage) {
      this.toastService.show('Session expired, please log in again', 'error', 5000);
      this.router.navigate(['/login']);
    }
  }

  private getTokenExpiryDate(): Date | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payloadBase64 = token.split('.')[1];
      const payloadJson = atob(payloadBase64);
      const payload = JSON.parse(payloadJson);

      if (typeof payload.exp !== 'number') return null;
      return new Date(payload.exp * 1000);
    } catch {
      return null;
    }
  }

  private scheduleAutoLogout() {
    this.clearAutoLogout();

    const expiryDate = this.getTokenExpiryDate();
    if (!expiryDate) return;

    const msUntilExpiry = expiryDate.getTime() - Date.now();

    if (msUntilExpiry <= 0) {
      this.handleSessionExpired();
      return;
    }

    this.logoutTimerId = setTimeout(() => this.handleSessionExpired(), msUntilExpiry);
  }

  private clearAutoLogout() {
    if (this.logoutTimerId !== null) {
      clearTimeout(this.logoutTimerId);
      this.logoutTimerId = null;
    }
  }
}
