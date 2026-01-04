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
    <div class="login-container">
      <h1>InsightMed Register</h1>
      
      <input 
        type="text" 
        placeholder="Email" 
        [(ngModel)]="email" 
        (input)="clearMessages()" 
      />
      
      <input 
        type="password" 
        placeholder="Password" 
        [(ngModel)]="password"
        (input)="clearMessages()" 
      />

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
      
      <button (click)="onRegister()" [disabled]="isLoading">
        {{ isLoading ? 'Registering...' : 'Register' }}
      </button>

      <div class="register-footer">
        Already have an account? <a routerLink="/login">Back to Login</a>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex; flex-direction: column; gap: 10px;
      width: 300px; margin: 100px auto; text-align: center;
      font-family: sans-serif;
    }
    input, button { padding: 10px; }
    
    .message {
      font-size: 0.9em;
      margin: 5px 0;
      padding: 10px 15px;
      border-radius: 4px;
      text-align: left;
    }

    ul {
      margin: 0;
      padding-left: 15px;
    }
    li {
      margin-bottom: 3px;
    }
    li:last-child {
      margin-bottom: 0;
    }

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
      margin-top: 15px;
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