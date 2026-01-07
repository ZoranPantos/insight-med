import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-container">
      
      <div class="header">
        <h2>Change Password</h2>
      </div>

      <div class="form-wrapper">
        
        <div class="form-group">
          <label>Old Password</label>
          <div class="password-wrapper">
            <input 
              [type]="showCurrentPassword ? 'text' : 'password'" 
              [(ngModel)]="currentPassword" 
              (input)="clearMessages()" 
              class="pill-input password-input" 
              placeholder="Current password"
            />
            
            <button 
              type="button" 
              class="toggle-password-btn" 
              (click)="toggleCurrent()"
              [attr.aria-label]="showCurrentPassword ? 'Hide password' : 'Show password'">
              
              <svg *ngIf="!showCurrentPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path>
                <line x1="1" y1="1" x2="23" y2="23"></line>
              </svg>

              <svg *ngIf="showCurrentPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                <circle cx="12" cy="12" r="3"></circle>
              </svg>
            </button>
          </div>
        </div>

        <div class="form-group">
          <label>New Password</label>
          <div class="password-wrapper">
            <input 
              [type]="showNewPassword ? 'text' : 'password'" 
              [(ngModel)]="newPassword" 
              (input)="clearMessages()" 
              class="pill-input password-input" 
              placeholder="New password"
            />
            
            <button 
              type="button" 
              class="toggle-password-btn" 
              (click)="toggleNew()"
              [attr.aria-label]="showNewPassword ? 'Hide password' : 'Show password'">
              
              <svg *ngIf="!showNewPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path>
                <line x1="1" y1="1" x2="23" y2="23"></line>
              </svg>

              <svg *ngIf="showNewPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                <circle cx="12" cy="12" r="3"></circle>
              </svg>
            </button>
          </div>
        </div>

        <div *ngIf="errorMessages.length > 0" class="message error-message">
          <ul>
            <li *ngFor="let msg of errorMessages">{{ msg }}</li>
          </ul>
        </div>

        <div class="actions">
          <button class="cancel-btn" (click)="onCancel()">Cancel</button>
          <button class="submit-btn" (click)="onSubmit()" [disabled]="isLoading">
            Update
          </button>
        </div>

      </div>
    </div>
  `,
  styles: [`
    .page-container { 
      padding: 20px 0; 
      font-family: sans-serif; 
      width: 320px; 
      margin: 0 auto; 
    }

    .header { 
      margin-bottom: 25px; 
      border-bottom: 1px solid #eee; 
      padding-bottom: 15px; 
    }
    h2 { margin: 0; color: #333; }

    .form-wrapper {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }

    .form-group { display: flex; flex-direction: column; gap: 8px; text-align: left; }
    label { font-size: 0.9em; font-weight: 500; color: #666; margin-left: 5px; }

    .pill-input {
      width: 100%; padding: 12px 20px; 
      border: 1px solid #ccc; border-radius: 25px; 
      outline: none; font-size: 1rem; box-sizing: border-box; 
      transition: border-color 0.2s, box-shadow 0.2s;
    }
    .pill-input:focus { border-color: #0078d4; box-shadow: 0 0 0 3px rgba(0, 120, 212, 0.15); }
    
    .password-wrapper { position: relative; width: 100%; }
    .password-input { padding-right: 50px; }

    .toggle-password-btn {
      position: absolute; right: 15px; top: 50%; transform: translateY(-50%);
      background: none; border: none; cursor: pointer; color: #888;
      padding: 0; display: flex; align-items: center; min-width: auto;
      transition: color 0.2s;
    }
    .toggle-password-btn:hover { color: #0078d4; }

    .message { 
      font-size: 0.9em;
      padding: 10px 15px; 
      border-radius: 12px; 
      text-align: left;
    }
    .error-message { background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
    ul { margin: 0; padding-left: 15px; }
    li { margin-bottom: 3px; }
    li:last-child { margin-bottom: 0; }

    .actions { display: flex; justify-content: space-between; gap: 15px; margin-top: 10px; }

    .submit-btn, .cancel-btn {
      flex: 1;
      padding: 10px 0; border: none; border-radius: 25px; 
      cursor: pointer; font-weight: 600; font-size: 1rem;
      transition: background-color 0.2s, transform 0.1s;
    }
    
    .submit-btn:active, .cancel-btn:active { 
      transform: scale(0.98); 
    }

    .cancel-btn { background-color: #e0e0e0; color: #333; }
    .cancel-btn:hover { background-color: #d0d0d0; }

    .submit-btn { background-color: #0078d4; color: white; }
    .submit-btn:hover:not(:disabled) { background-color: #005a9e; }
    .submit-btn:disabled { background-color: #a0cce8; cursor: not-allowed; }
  `]
})
export class ChangePasswordComponent {
  private http = inject(HttpClient);
  private router = inject(Router);
  private cd = inject(ChangeDetectorRef);

  currentPassword = '';
  newPassword = '';

  showCurrentPassword = false;
  showNewPassword = false;

  isLoading = false;
  errorMessages: string[] = [];

  toggleCurrent() { this.showCurrentPassword = !this.showCurrentPassword; }
  toggleNew() { this.showNewPassword = !this.showNewPassword; }

  clearMessages() {
    this.errorMessages = [];
  }

  onCancel() {
    this.router.navigate(['/profile']);
  }

  onSubmit() {
    if (!this.currentPassword || !this.newPassword) {
      this.errorMessages = ['Please fill in all fields'];
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const payload = {
      currentPassword: this.currentPassword,
      newPassword: this.newPassword
    };

    this.http.post('http://localhost:5000/api/Auth/changePassword', payload, { responseType: 'text' })
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/profile']);
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.isLoading = false;
          
          if (err.error) {
             try {
                const errorObj = JSON.parse(err.error);
                if (errorObj && errorObj.detail) {
                    const rawMessages = errorObj.detail.split(',');
                    this.errorMessages = rawMessages.map((msg: string) => msg.trim().replace(/\.$/, ''));
                } else {
                    this.errorMessages = ['Failed to change password'];
                }
             } catch (e) {
                this.errorMessages = [err.error]; 
             }
          } else {
             this.errorMessages = ['Failed to change password'];
          }
          
          this.cd.detectChanges();
        }
      });
  }
}