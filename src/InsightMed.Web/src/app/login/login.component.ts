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
        
        <input 
          type="password" 
          placeholder="Password" 
          [(ngModel)]="password"
          (input)="clearMessages()" 
          class="pill-input"
        />
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
    
    .pill-input:focus {
      border-color: #0078d4;
      box-shadow: 0 0 0 3px rgba(0, 120, 212, 0.15);
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

    /* UPDATED: Changed to 0.98 to match Profile buttons exactly */
    .login-btn:active {
      transform: scale(0.98);
    }
    
    .message {
      font-size: 0.9em;
      padding: 10px 15px;
      border-radius: 12px; 
      text-align: left;
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
  
  errorMessages: string[] = []; 

  constructor(
    private router: Router,
    private authService: AuthService,
    private cd: ChangeDetectorRef
  ) {}

  clearMessages() {
    this.errorMessages = [];
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