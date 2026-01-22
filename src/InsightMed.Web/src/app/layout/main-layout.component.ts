import { Component, inject, ChangeDetectorRef, effect, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, ActivatedRoute } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { SignalrService } from '../services/signalr.service';
import { LoadingSpinnerComponent } from '../shared/loading-spinner.component';
import { ErrorDisplayComponent } from '../shared/error-display.component';
import { ToastComponent } from '../shared/toast.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, FormsModule, LoadingSpinnerComponent, ErrorDisplayComponent, ToastComponent, DatePipe],
  template: `
    <div class="app-container">
      <nav class="navbar">
        <div class="nav-group left">
          <a routerLink="/reports" routerLinkActive="active-link">Reports</a>
          <a routerLink="/requests" routerLinkActive="active-link">Requests</a>
          <a routerLink="/patients" routerLinkActive="active-link">Patients</a>
        </div>

        <div class="nav-group center" *ngIf="!isSearchHidden">
          <input type="text" placeholder="Search..." class="search-input" 
                 [(ngModel)]="searchTerm" (keyup.enter)="onSearch()" />
          <button class="search-btn" (click)="onSearch()">Go</button>
        </div>

        <div class="nav-group center" *ngIf="isSearchHidden"></div>

        <div class="nav-group right">
          <div class="notification-wrapper">
            <span (click)="toggleNotifications()" 
                  class="notification-trigger" 
                  [class.active-trigger]="isNotificationsOpen">
              Notifications
              <span *ngIf="signalrService.hasUnseenNotifications()" class="badge"></span>
            </span>

            <div *ngIf="isNotificationsOpen" class="dropdown">
              <div class="dropdown-content">
                
                <app-loading-spinner 
                  *ngIf="isLoading" 
                  minHeight="150px">
                </app-loading-spinner>

                <app-error-display
                  *ngIf="errorMessage && !isLoading"
                  [message]="errorMessage"
                  minHeight="150px">
                </app-error-display>

                <ng-container *ngIf="!isLoading && !errorMessage">
                  <div *ngFor="let item of notifications" 
                       class="notification-item"
                       [class.clickable]="item.labReportId"
                       (click)="onNotificationClick(item)">
                    
                    <div class="notif-text">
                        <ng-container *ngIf="item.parsedText.hasMatch; else plainText">
                            {{ item.parsedText.prefix }}
                            <span class="highlight-text">
                                {{ item.parsedText.name }}
                                <span *ngIf="item.parsedText.uid" class="uid-text">{{ item.parsedText.uid }}</span>
                            </span>
                            {{ item.parsedText.suffix }}
                        </ng-container>
                        
                        <ng-template #plainText>
                            {{ item.displayMessage }}
                        </ng-template>
                    </div>
                    
                    <div *ngIf="item.displayDate" class="notification-date">
                      {{ item.displayDate | date:'MMM d, y, h:mm a' }}
                    </div>

                  </div>

                  <div *ngIf="notifications.length === 0" class="empty-state">
                    No new notifications
                  </div>
                </ng-container>

              </div>
              
              <div class="dropdown-footer" *ngIf="notifications.length > 0 && !isLoading">
                <button class="clear-btn" (click)="clearAll()">Clear</button>
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

      <app-toast></app-toast>
    </div>
  `,
  styles: [`
    .app-container { width: 66%; margin: 0 auto; min-width: 800px; }
    .navbar { display: flex; justify-content: space-between; align-items: center; padding: 15px 0; }
    .nav-group { display: flex; align-items: center; gap: 10px; }
    
    .nav-group.center { flex-grow: 1; justify-content: center; } 

    a { 
      text-decoration: none; 
      color: #666;
      font-weight: 500; 
      cursor: pointer; 
      padding: 8px 12px; 
      position: relative;
      transition: color 0.2s;
    }

    a:hover { 
      color: #0078d4; 
      background-color: transparent;
    }

    .active-link { 
      color: #0078d4 !important; 
      font-weight: 600;
    }
    
    .active-link::after {
      content: '';
      position: absolute;
      bottom: -4px;
      left: 0;
      width: 100%;
      height: 2px;
      background-color: #0078d4;
      border-radius: 2px;
    }

    .search-input {
      width: 300px; 
      padding: 8px 15px;
      border: 1px solid #ccc;
      border-radius: 20px; 
      outline: none;
      font-size: 0.95rem;
      transition: border-color 0.2s, box-shadow 0.2s;
    }
    .search-input:focus {
      border-color: #0078d4;
      box-shadow: 0 0 0 2px rgba(0, 120, 212, 0.2);
    }

    .search-btn {
      padding: 8px 20px;
      background-color: #0078d4; 
      color: white;
      border: none;
      border-radius: 20px; 
      cursor: pointer;
      font-weight: 600; 
      transition: background-color 0.2s, transform 0.1s; 
    }

    .search-btn:hover {
      background-color: #005a9e; 
    }

    .search-btn:active {
      transform: scale(0.95);
    }

    .notification-wrapper { position: relative; cursor: pointer; margin-right: 15px; }
    
    .notification-trigger { 
      user-select: none; 
      font-weight: 500; 
      color: #666; 
      transition: color 0.2s; 
    }
    .notification-trigger:hover { color: #0078d4; }

    .active-trigger {
      color: #0078d4;
      font-weight: 600;
    }

    .badge {
      display: inline-block; width: 8px; height: 8px; 
      background-color: red; border-radius: 50%; 
      margin-left: 5px; vertical-align: top;
    }

    .dropdown {
      position: absolute;
      top: 35px; right: 0; 
      width: 400px;
      background: white; border: 1px solid #ccc;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15); 
      border-radius: 8px; 
      z-index: 1000;
      overflow: hidden;
    }

    .dropdown-content { max-height: 250px; overflow-y: auto; }
    
    .notification-item { 
      padding: 12px 16px; 
      border-bottom: 1px solid #f0f0f0; 
      font-size: 0.9rem; 
      color: #333; 
    }
    .notification-item:hover { background-color: #f9f9f9; }
    
    .clickable { cursor: pointer; } 

    .highlight-text {
      font-weight: 500;
      color: #222;
    }

    .uid-text {
      color: #3b5998;
      margin-left: 3px;
    }

    .notification-date {
      font-size: 0.85em;
      color: #888;
      margin-top: 4px;
    }

    .empty-state { 
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 150px;
      padding: 20px; 
      color: #999; 
      font-style: italic; 
    }

    .dropdown-footer {
      padding: 15px; 
      text-align: center; 
      background-color: white; 
      border-top: 1px solid #eee;
    }

    .clear-btn {
      padding: 8px 20px; 
      font-size: 0.95rem;
      background-color: #dc3545; 
      color: white;
      border: none;
      border-radius: 25px; 
      cursor: pointer;
      font-weight: 600;
      transition: background-color 0.2s, transform 0.1s; 
    }

    .clear-btn:hover {
      background-color: #bb2d3b; 
    }

    .clear-btn:active {
      transform: scale(0.95);
    }
    
    hr { margin: 0; border: 0; border-top: 1px solid #eee; }
    main { padding-top: 20px; }
  `]
})
export class MainLayoutComponent implements OnInit, OnDestroy {
  http = inject(HttpClient);
  cdr = inject(ChangeDetectorRef);
  signalrService = inject(SignalrService);
  router = inject(Router);
  route = inject(ActivatedRoute);

  searchTerm = '';
  
  isNotificationsOpen = false;
  isLoading = false;
  errorMessage = '';
  notifications: any[] = [];

  constructor() {
    effect(() => {
      this.cdr.detectChanges(); 
    });
  }

  get isSearchHidden(): boolean {
    return this.router.url.startsWith('/profile') || 
           this.router.url.startsWith('/change-password') ||
           this.router.url.startsWith('/patients/') ||
           this.router.url.startsWith('/requests/create') ||
           this.router.url.startsWith('/reports/');
  }

  onSearch() {
    console.log('Search triggered with term:', this.searchTerm); 
    const currentPath = this.router.url.split('?')[0];

    this.router.navigate([currentPath], {
      queryParams: { 
        searchKey: this.searchTerm || null,
        pageNumber: 1
      },
      queryParamsHandling: 'merge'
    });
  }

  ngOnInit() {
    this.signalrService.startConnection();
  }

  ngOnDestroy() {
    this.signalrService.stopConnection();
  }

  toggleNotifications() {
    this.isNotificationsOpen = !this.isNotificationsOpen;

    if (this.isNotificationsOpen) {
      this.fetchNotifications();
    }
  }

  fetchNotifications() {
    this.isLoading = true;
    this.errorMessage = ''; 
    this.notifications = []; 

    this.http.get<any>('http://localhost:5000/api/Notifications', {
      params: { filter: 'Unseen' }
    }).subscribe({
      next: (data) => {
        this.notifications = data.notifications.map((n: any) => this.parseNotification(n));
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load data';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  parseNotification(notification: any) {
    const separator = 'Date created: ';
    let textPart = notification.message;
    let datePart = null;

    if (notification.message && notification.message.includes(separator)) {
      const parts = notification.message.split(separator);
      textPart = parts[0];
      datePart = parts[1];
    }

    const nameRegex = /Report for patient (.*?) is available/;
    const match = textPart.match(nameRegex);

    let parsedText = {
      hasMatch: false,
      prefix: textPart,
      name: '',
      uid: '',
      suffix: ''
    };

    if (match && match[1]) {
      const fullIdentity = match[1];
      
      const uidIndex = fullIdentity.lastIndexOf('UID-');
      
      let nameStr = fullIdentity;
      let uidStr = '';

      if (uidIndex > -1) {
        nameStr = fullIdentity.substring(0, uidIndex).trim();
        uidStr = fullIdentity.substring(uidIndex).trim();
      }

      parsedText = {
        hasMatch: true,
        prefix: 'Report for patient ',
        name: nameStr,
        uid: uidStr,
        suffix: ' is available.'
      };
    }

    return {
      ...notification,
      displayMessage: textPart,
      parsedText: parsedText,
      displayDate: datePart ? new Date(datePart + ' UTC') : null
    };
  }

  clearAll() {
    if (this.notifications.length === 0) return;
    const ids = this.notifications.map(n => n.id);

    this.http.put('http://localhost:5000/api/Notifications/seen', ids)
        .subscribe({
          next: () => {
            this.notifications = [];
            this.signalrService.hasUnseenNotifications.set(false);
            this.cdr.detectChanges();
          },
          error: (err) => console.error(err)
        });
  }

  onNotificationClick(item: any) {
    if (item.labReportId) {
      this.router.navigate(['/reports', item.labReportId]);
      this.isNotificationsOpen = false;
    }
  }
}