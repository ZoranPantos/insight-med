import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  
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
}