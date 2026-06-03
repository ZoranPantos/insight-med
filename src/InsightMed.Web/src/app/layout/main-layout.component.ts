import { Component, inject, ChangeDetectorRef, effect, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, ActivatedRoute } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { SignalrService } from '../services/signalr.service';
import { LoadingSpinnerComponent } from '../shared/loading-spinner/loading-spinner.component';
import { ErrorDisplayComponent } from '../shared/error-display/error-display.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, FormsModule, LoadingSpinnerComponent, ErrorDisplayComponent, DatePipe],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
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

    this.http.get<any>('/api/Notifications', {
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

    this.http.put('/api/Notifications/seen', ids)
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
