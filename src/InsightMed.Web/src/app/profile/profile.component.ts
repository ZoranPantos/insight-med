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
    <div class="profile-container">
      <h2>My Profile</h2>

      <div *ngIf="isLoading">Loading profile...</div>

      <div *ngIf="!isLoading && accountInfo" class="profile-card">
        <div class="info-group">
          <label>Username</label>
          <div class="value">{{ accountInfo.userName }}</div>
        </div>

        <div class="info-group">
          <label>Email</label>
          <div class="value">{{ accountInfo.email }}</div>
        </div>

        <div class="info-group">
          <label>Status</label>
          <div class="value">
            <span [class.verified]="accountInfo.emailConfirmed" class="status-badge">
              {{ accountInfo.emailConfirmed ? 'Verified' : 'Unverified' }}
            </span>
          </div>
        </div>
      </div>

      <hr />

      <button class="logout-btn" (click)="onLogout()">Log Out</button>
    </div>
  `,
  styles: [`
    .profile-container { width: 400px; margin: 0 auto; font-family: sans-serif; }
    h2 { border-bottom: 2px solid #f0f0f0; padding-bottom: 10px; margin-bottom: 20px; }
    .profile-card { background: #f9f9f9; padding: 20px; border-radius: 8px; border: 1px solid #ddd; }
    .info-group { margin-bottom: 15px; }
    .info-group label { display: block; font-size: 0.85em; color: #666; margin-bottom: 4px; }
    .info-group .value { font-size: 1.1em; font-weight: 500; }
    .status-badge { font-size: 0.8em; padding: 4px 8px; border-radius: 4px; background: #eee; color: #555; }
    .status-badge.verified { background: #d4edda; color: #155724; }
    hr { margin: 30px 0; border: 0; border-top: 1px solid #eee; }
    .logout-btn { width: 100%; padding: 12px; background-color: #dc3545; color: white; border: none; border-radius: 4px; font-size: 1rem; cursor: pointer; transition: background 0.2s; }
    .logout-btn:hover { background-color: #c82333; }
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

  onLogout() {
    this.authService.logout();
  }
}