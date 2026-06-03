import { Component, ChangeDetectorRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  email = '';
  password = '';
  confirmPassword = '';
  
  showPassword = false;
  showConfirmPassword = false;
  
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

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  onRegister() {
    if (!this.email || !this.password || !this.confirmPassword) {
      this.errorMessages = ['All fields are required'];
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessages = ['Passwords do not match'];
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
        this.confirmPassword = '';
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
