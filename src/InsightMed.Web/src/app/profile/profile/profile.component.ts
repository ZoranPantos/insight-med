import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, NavigationEnd } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Subscription, filter } from 'rxjs';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { ErrorDisplayComponent } from '../../shared/error-display/error-display.component';

interface AccountInfo {
  userName: string;
  email: string;
  emailConfirmed: boolean;
  passwordLastChanged: string;
}

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, DatePipe, LoadingSpinnerComponent, ErrorDisplayComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private cd = inject(ChangeDetectorRef);
  private router = inject(Router);

  accountInfo: AccountInfo | null = null;
  isLoading = false;
  errorMessage = '';
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
      this.errorMessage = 'User ID not found. Please log in again.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    
    this.http.get<AccountInfo>(`/api/Auth/accountInfo/${id}`)
      .subscribe({
        next: (data) => {
          this.accountInfo = data;
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error('Failed to load data', err);
          this.errorMessage = 'Failed to load data';
          this.isLoading = false;
          this.cd.detectChanges();
        }
      });
  }

  onChangePassword() {
    this.router.navigate(['/change-password']);
  }

  onLogout() {
    this.authService.logout();
  }
}
