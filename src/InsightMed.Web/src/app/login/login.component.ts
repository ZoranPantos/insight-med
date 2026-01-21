import { Component, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router'; 
import { FormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common'; 
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink], 
  template: `
    <div class="login-container">
      <h1>InsightMed Login</h1>
      
      <div class="form-group">
        <input 
          type="text" 
          placeholder="Email" 
          [(ngModel)]="email" 
          (input)="clearMessages()" 
          class="pill-input"
        />
        
        <div class="password-wrapper">
          <input 
            [type]="showPassword ? 'text' : 'password'" 
            placeholder="Password" 
            [(ngModel)]="password"
            (input)="clearMessages()" 
            class="pill-input password-input"
          />
          
          <button 
            type="button" 
            class="toggle-password-btn" 
            (click)="togglePasswordVisibility()"
            [attr.aria-label]="showPassword ? 'Hide password' : 'Show password'">
            
            <svg *ngIf="!showPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path>
              <line x1="1" y1="1" x2="23" y2="23"></line>
            </svg>

            <svg *ngIf="showPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
              <circle cx="12" cy="12" r="3"></circle>
            </svg>

          </button>
        </div>

      </div>

      <div *ngIf="errorMessages.length > 0" class="message error-message">
        <ul>
          <li *ngFor="let msg of errorMessages">
            {{ msg }}
          </li>
        </ul>
      </div>
      
      <button class="login-btn" (click)="onLogin()">Login</button>

      <div class="register-footer">
        Don't have an account? <a routerLink="/register">Register here</a>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex; flex-direction: column; gap: 25px;
      width: 320px; margin: 100px auto; text-align: center;
      font-family: sans-serif;
    }
    
    h1 { margin-bottom: 10px; color: #333; }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 15px;
    }

    .password-wrapper {
      position: relative;
      width: 100%;
    }

    .pill-input {
      width: 100%;
      padding: 12px 20px; 
      border: 1px solid #ccc;
      border-radius: 25px; 
      outline: none;
      font-size: 1rem;
      box-sizing: border-box; 
      transition: border-color 0.2s, box-shadow 0.2s;
    }
    
    .password-input {
      padding-right: 50px; 
    }

    .pill-input:focus {
      border-color: #0078d4;
      box-shadow: 0 0 0 3px rgba(0, 120, 212, 0.15);
    }

    .toggle-password-btn {
      position: absolute;
      right: 15px;
      top: 50%;
      transform: translateY(-50%);
      background: none;
      border: none;
      cursor: pointer;
      color: #888;
      padding: 0;
      display: flex;
      align-items: center;
      transition: color 0.2s;
    }

    .toggle-password-btn:hover {
      color: #0078d4;
    }

    .login-btn {
      width: auto;
      align-self: center; 
      min-width: 140px; 
      padding: 10px 24px;
      
      background-color: #0078d4;
      color: white;
      border: none;
      border-radius: 25px; 
      cursor: pointer;
      font-size: 1rem;
      font-weight: 600; 
      
      transition: background-color 0.2s, transform 0.1s;
    }

    .login-btn:hover {
      background-color: #005a9e;
    }

    .login-btn:active {
      transform: scale(0.98);
    }
    
    .message {
      padding: 10px 15px;
      border-radius: 12px; 
      text-align: left;
      box-sizing: border-box;
    }

    ul { margin: 0; padding-left: 15px; }
    li { margin-bottom: 3px; }
    li:last-child { margin-bottom: 0; }

    .error-message {
      color: #721c24;
      background-color: #f8d7da;
      border: 1px solid #f5c6cb;
    }

    .register-footer {
      margin-top: 10px;
      font-size: 0.9em;
      color: #666;
    }
    .register-footer a {
      color: #0078d4;
      text-decoration: none;
      font-weight: 500;
      cursor: pointer;
    }
    .register-footer a:hover {
      text-decoration: underline;
    }
  `]
})
export class LoginComponent {
  email = '';
  password = '';
  showPassword = false;
  
  errorMessages: string[] = []; 

  constructor(
    private router: Router,
    private authService: AuthService,
    private cd: ChangeDetectorRef
  ) {}

  clearMessages() {
    this.errorMessages = [];
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  onLogin() {
    if (!this.email || !this.password) {
      this.errorMessages = ['Email or password is missing'];
      return; 
    }

    this.clearMessages();

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
           const rawMessages = err.error.detail.split(',');
           this.errorMessages = rawMessages.map((msg: string) => msg.trim().replace(/\.$/, ''));
        } else if (err.status === 401) {
           this.errorMessages = ['Invalid credentials'];
        } else {
           this.errorMessages = ['Login failed'];
        }

        this.cd.detectChanges();
      }
    });
  }
}