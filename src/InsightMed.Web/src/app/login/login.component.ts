import { Component, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router'; 
import { FormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common'; 
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink], 
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
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
