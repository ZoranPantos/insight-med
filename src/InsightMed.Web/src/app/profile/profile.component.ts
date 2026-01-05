import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, NavigationEnd } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Subscription, filter } from 'rxjs';

interface AccountInfo {
  userName: string;
  email: string;
  emailConfirmed: boolean;
}

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-container">
      <div class="header">
        <h2>Profile</h2>
      </div>

      <div *ngIf="isLoading" class="loading">
        Loading profile...
      </div>

      <div *ngIf="!isLoading && accountInfo" class="content-wrapper">
        
        <div class="details-card">
          <div class="info-row">
            <label>Username</label>
            <div class="value">{{ accountInfo.userName }}</div>
          </div>

          <div class="info-row">
            <label>Email Address</label>
            <div class="value">{{ accountInfo.email }}</div>
          </div>

          <div class="info-row">
            <label>Account Status</label>
            <div class="value">
              <span [class.verified]="accountInfo.emailConfirmed" class="status-badge">
                {{ accountInfo.emailConfirmed ? 'Verified' : 'Unverified' }}
              </span>
            </div>
          </div>
        </div>

        <div class="actions">
          <button class="change-pw-btn" (click)="onChangePassword()">Change Password</button>
          <button class="logout-btn" (click)="onLogout()">Log Out</button>
        </div>

      </div>
    </div>
  `,
  styles: [`
    .page-container { 
      padding: 20px 0; 
      font-family: sans-serif; 
      max-width: 600px; 
      margin: 0 auto; 
    }

    .header { 
      margin-bottom: 30px; 
    }
    
    h2 { margin: 0; color: #333; }

    .content-wrapper {
      display: flex;
      flex-direction: column;
      gap: 30px;
    }

    .info-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 15px 0;
      border-bottom: 1px solid #f5f5f5;
    }
    
    .info-row:last-child {
      border-bottom: none;
    }

    .info-row label {
      color: #666;
      font-weight: 500;
      font-size: 0.95rem;
    }

    .info-row .value {
      font-weight: 600;
      color: #333;
      font-size: 1rem;
    }

    .status-badge {
      display: inline-block;
      padding: 4px 12px;
      border-radius: 12px;
      font-size: 0.85em;
      font-weight: 500;
      background: #eee; 
      color: #555;
    }
    .status-badge.verified {
      background: #d4edda; 
      color: #155724;
    }

    .actions {
      margin-top: 10px;
      display: flex;
      gap: 15px; 
    }

    button {
      padding: 10px 24px;
      border: none;
      border-radius: 20px; /* Pill Shape */
      cursor: pointer;
      
      font-weight: 600; 
      
      font-size: 0.95rem;
      transition: background-color 0.2s, transform 0.1s;
      min-width: 140px;
    }
    
    button:active {
      transform: scale(0.98);
    }

    .change-pw-btn {
      background-color: #e0e0e0;
      color: #333;
    }
    .change-pw-btn:hover {
      background-color: #d0d0d0;
    }

    .logout-btn {
      background-color: #dc3545;
      color: white;
    }
    .logout-btn:hover {
      background-color: #bb2d3b;
    }

    .loading { padding: 20px; text-align: center; color: #666; }
  `]
})
export class ProfileComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private cd = inject(ChangeDetectorRef);
  private router = inject(Router);

  accountInfo: AccountInfo | null = null;
  isLoading = false;
  private routerSubscription: Subscription | undefined;

  constructor() {
    this.routerSubscription = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.fetchProfile();
    });
  }

  ngOnInit() {
    this.fetchProfile();
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  fetchProfile() {
    const id = this.authService.getUserIdFromToken();

    if (!id) {
      console.error('Cannot fetch profile: User ID not found in token');
      return;
    }

    this.isLoading = true;
    
    this.http.get<AccountInfo>(`http://localhost:5000/api/Auth/accountInfo/${id}`)
      .subscribe({
        next: (data) => {
          this.accountInfo = data;
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error('Failed to load profile', err);
          this.isLoading = false;
          this.cd.detectChanges();
        }
      });
  }

  onChangePassword() {
    console.log('Navigate to change password');
  }

  onLogout() {
    this.authService.logout();
  }
}