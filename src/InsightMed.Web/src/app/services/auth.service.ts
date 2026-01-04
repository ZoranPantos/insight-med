import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  
  private apiUrl = 'http://localhost:5000/api'; 
  private tokenKey = 'insight_med_token';

  login(payload: any) {
    return this.http.post<{ token: string }>(`${this.apiUrl}/Auth/login`, payload)
      .pipe(
        tap(response => {
          localStorage.setItem(this.tokenKey, response.token);
        })
      );
  }

  logout() {
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
    return !!this.getToken();
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
}