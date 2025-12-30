import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  template: `
    <div class="login-container">
      <h1>InsightMed Login</h1>
      <input type="text" placeholder="Username" />
      <input type="password" placeholder="Password" />
      <button (click)="onLogin()">Login</button>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex; flex-direction: column; gap: 10px;
      width: 300px; margin: 100px auto; text-align: center;
    }
    input, button { padding: 10px; }
  `]
})
export class LoginComponent {
  // We inject the Router so we can navigate via code
  constructor(private router: Router) {}

  onLogin() {
    // Navigate to the main app (Reports)
    this.router.navigate(['/reports']);
  }
}