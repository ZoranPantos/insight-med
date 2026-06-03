import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent {
  private http = inject(HttpClient);
  private router = inject(Router);
  private cd = inject(ChangeDetectorRef);
  private toastService = inject(ToastService);

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

    this.http.post('/api/Auth/changePassword', payload, { responseType: 'text' })
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.toastService.show('Action successful', 'success');
          this.router.navigate(['/profile']);
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.isLoading = false;
          
          this.toastService.show('Action failed', 'error');
          
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
