import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common'; 
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule], 
  template: `
    <div class="login-container">
      <h1>InsightMed Login</h1>
      
      <input 
        type="text" 
        placeholder="Email" 
        [(ngModel)]="email" 
        (input)="errorMessage = ''" 
      />
      
      <input 
        type="password" 
        placeholder="Password" 
        [(ngModel)]="password"
        (input)="errorMessage = ''" 
      />

      <p *ngIf="errorMessage" class="error-message">
        {{ errorMessage }}
      </p>
      
      <button (click)="onLogin()">Login</button>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex; flex-direction: column; gap: 10px;
      width: 300px; margin: 100px auto; text-align: center;
      font-family: sans-serif;
    }
    input, button { padding: 10px; }
    
    .error-message {
      color: red;
      font-size: 0.9em;
      margin: 0;
    }
  `]
})
export class LoginComponent {
  email = '';
  password = '';
  errorMessage = '';

  constructor(
    private router: Router,
    private authService: AuthService,
    private cd: ChangeDetectorRef
  ) {}

  onLogin() {
    if (!this.email || !this.password) {
      this.errorMessage = 'Email or password missing';
      return; 
    }

    const payload = {
      email: this.email,
      password: this.password
    };

    this.authService.login(payload).subscribe({
      next: () => {
        this.router.navigate(['/reports']);
      },
      error: (err) => {
        if (err.error && err.error.detail) {
          this.errorMessage = err.error.detail;
        } else if (err.status === 401) {
          this.errorMessage = 'Invalid credentials';
        } else {
          this.errorMessage = 'Login failed';
        }

        this.cd.detectChanges();
      }
    });
  }
}