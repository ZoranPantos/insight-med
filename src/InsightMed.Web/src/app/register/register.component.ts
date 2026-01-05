import { Component, ChangeDetectorRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  template: `
    <div class="register-container">
      <h1>InsightMed Register</h1>
      
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

      <div *ngIf="successMessage" class="message success-message">
        {{ successMessage }}
      </div>
      
      <button class="register-btn" (click)="onRegister()" [disabled]="isLoading">
        {{ isLoading ? 'Registering...' : 'Register' }}
      </button>

      <div class="register-footer">
        Already have an account? <a routerLink="/login">Back to Login</a>
      </div>
    </div>
  `,
  styles: [`
    .register-container {
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

    .register-btn {
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

    .register-btn:hover:not(:disabled) {
      background-color: #005a9e;
    }

    .register-btn:active:not(:disabled) {
      transform: scale(0.98);
    }

    .register-btn:disabled {
      background-color: #a0cce8;
      cursor: not-allowed;
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

    .success-message {
      color: #155724;
      background-color: #d4edda;
      border: 1px solid #c3e6cb;
      text-align: center;
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
export class RegisterComponent {
  email = '';
  password = '';
  
  errorMessages: string[] = []; 
  successMessage = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private cd: ChangeDetectorRef
  ) {}

  clearMessages() {
    this.errorMessages = [];
    this.successMessage = '';
  }

  onRegister() {
    if (!this.email || !this.password) {
      this.errorMessages = ['Email or password is missing'];
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const payload = {
      email: this.email,
      password: this.password
    };

    this.authService.register(payload).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Registration successful!';
        this.email = '';
        this.password = '';
        this.cd.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);

        if (err.error && err.error.detail) {
          const rawMessages = err.error.detail.split(',');
          this.errorMessages = rawMessages.map((msg: string) => {
             return msg.trim().replace(/\.$/, '');
          });
        } else {
          this.errorMessages = ['Registration failed'];
        }
        
        this.cd.detectChanges();
      }
    });
  }
}