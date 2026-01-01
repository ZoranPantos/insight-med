import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common'; // Needed for *ngIf and *ngFor
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  template: `
    <div class="app-container">
      <nav class="navbar">
        <div class="nav-group left">
          <a routerLink="/reports" routerLinkActive="active-link">Reports</a>
          <a routerLink="/requests" routerLinkActive="active-link">Requests</a>
          <a routerLink="/patients" routerLinkActive="active-link">Patients</a>
        </div>

        <div class="nav-group center">
          <input type="text" placeholder="Search..." />
          <button>Go</button>
        </div>

        <div class="nav-group right">
          
          <div class="notification-wrapper">
            <span (click)="toggleNotifications()" class="notification-trigger">
              Notifications
              <span *ngIf="notifications.length > 0" class="badge"></span>
            </span>

            <div *ngIf="isNotificationsOpen" class="dropdown">
              
              <div class="dropdown-content">
                <div *ngFor="let item of notifications" class="notification-item">
                  {{ item.message }}
                </div>
                
                <div *ngIf="notifications.length === 0" class="empty-state">
                  No notifications
                </div>
              </div>

              <div class="dropdown-footer" (click)="clearAll()">
                Clear All
              </div>
            </div>
          </div>

          <a routerLink="/profile" routerLinkActive="active-link">Profile</a>
        </div>
      </nav>

      <hr />

      <main>
        <router-outlet /> 
      </main>
    </div>
  `,
  styles: [`
    .app-container { width: 66%; margin: 0 auto; min-width: 800px; }
    .navbar { display: flex; justify-content: space-between; align-items: center; padding: 15px 0; }
    .nav-group { display: flex; align-items: center; gap: 10px; }
    
    a { text-decoration: none; color: black; font-weight: 500; cursor: pointer; padding: 8px 16px; border-radius: 4px; transition: all 0.2s; }
    a:hover { background-color: #e6e6e6; }
    .active-link { background-color: #0078d4; color: white !important; }

    /* NOTIFICATION STYLES */
    .notification-wrapper { position: relative; cursor: pointer; }
    .notification-trigger { user-select: none; font-weight: 500; }
    
    .badge {
      display: inline-block; width: 8px; height: 8px; 
      background-color: red; border-radius: 50%; 
      margin-left: 5px; vertical-align: top;
    }

    /* Floating Dropdown */
    .dropdown {
      position: absolute;
      top: 35px; right: 0; width: 300px;
      background: white; border: 1px solid #ccc;
      box-shadow: 0 4px 8px rgba(0,0,0,0.15);
      border-radius: 6px; z-index: 1000;
      overflow: hidden; /* Ensures footer corners are clipped */
    }

    /* Scrollable Area */
    .dropdown-content {
      max-height: 200px; /* Limits height to allow scrolling */
      overflow-y: auto;
    }

    .notification-item {
      padding: 12px; border-bottom: 1px solid #f0f0f0; font-size: 0.9rem;
    }
    .notification-item:hover { background-color: #f9f9f9; }

    .empty-state { padding: 20px; text-align: center; color: #999; font-style: italic; }

    /* Footer */
    .dropdown-footer {
      padding: 10px; text-align: center; background-color: #f5f5f5;
      color: #d9534f; font-weight: bold; cursor: pointer;
      border-top: 1px solid #ddd;
    }
    .dropdown-footer:hover { background-color: #e8e8e8; }
  `]
})
export class MainLayoutComponent {
  http = inject(HttpClient);
  
  isNotificationsOpen = false;
  notifications: any[] = [];

  toggleNotifications() {
    this.isNotificationsOpen = !this.isNotificationsOpen;
    
    // Fetch only if opening and empty
    if (this.isNotificationsOpen && this.notifications.length === 0) {
      this.fetchNotifications();
    }
  }

  fetchNotifications() {
  // We pass the parameters as a second argument to .get()
  this.http.get<any>('http://localhost:5000/api/Notifications', {
      params: { filter: 'Unseen' }
    }).subscribe({
      next: (data) => {
        this.notifications = data.notifications; 
      },
      error: (err) => console.error('Failed to load notifications', err)
    });
  }

  clearAll() {
    this.notifications = [];
    // Optional: Close dropdown automatically
    // this.isNotificationsOpen = false; 
  }
}