import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, NavigationEnd, RouterLink } from '@angular/router'; 
import { Subscription, filter } from 'rxjs';

interface LabParameter {
  name: string;
}

interface LabRequest {
  id: number;
  created: string;
  labRequestState: number;
  patientFullName: string;
  patientUid: string;
  labParameters: LabParameter[];
}

interface LabRequestsResponse {
  labRequests: LabRequest[];
}

@Component({
  selector: 'app-requests',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink], 
  template: `
    <div class="page-container">
      <div class="header">
        <h2>Lab Requests</h2>
      </div>

      <div *ngIf="isLoading" class="loading">
        Loading requests...
      </div>

      <div *ngIf="errorMessage" class="error">
        {{ errorMessage }}
      </div>

      <div *ngIf="!isLoading && !errorMessage" class="table-container">
        <table>
          <thead>
            <tr>
              <th>Date Created</th>
              <th>Patient</th>
              <th>Status</th>
              <th>Parameters</th>
              <th class="actions-col">Actions</th> 
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let req of requests">
              <td class="date-col">
                {{ req.created | date:'MMM d, y, h:mm a' }}
              </td>

              <td>
                <div class="patient-name">{{ req.patientFullName }}</div>
                <div class="patient-uid">{{ req.patientUid }}</div>
              </td>

              <td>
                <span class="status-badge" 
                      [class.pending]="req.labRequestState === 0"
                      [class.completed]="req.labRequestState === 1">
                  {{ req.labRequestState === 1 ? 'Completed' : 'Pending' }}
                </span>
              </td>

              <td>
                <ol class="param-list">
                  <li *ngFor="let param of req.labParameters">
                    {{ param.name }}
                  </li>
                </ol>
              </td>

              <td class="actions-col">
                <a *ngIf="req.labRequestState === 1" 
                   [routerLink]="['/reports', req.id]" 
                   class="view-report-link">
                   View Report
                </a>
                
                <span *ngIf="req.labRequestState === 0" class="no-action">
                  -
                </span>
              </td>
            </tr>

            <tr *ngIf="requests.length === 0">
              <td colspan="5" class="empty-text">No requests found.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    h2 { margin: 0; color: #333; }

    .table-container { border: 1px solid #e0e0e0; border-radius: 4px; overflow: hidden; }
    table { width: 100%; border-collapse: collapse; background: white; }
    th, td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #f0f0f0; vertical-align: top; }
    
    th {
      background-color: #fafafa; font-weight: 600; color: #555;
      font-size: 0.9em; text-transform: uppercase; letter-spacing: 0.5px;
    }
    tr:hover { background-color: #f9f9f9; }

    .date-col { white-space: nowrap; color: #666; font-size: 0.9rem; }
    .patient-name { font-weight: 500; }
    .patient-uid { font-size: 0.8em; color: #888; margin-top: 2px; }

    .status-badge {
      display: inline-block; padding: 4px 10px; border-radius: 12px;
      font-size: 0.85em; font-weight: 500;
    }
    .status-badge.pending { background-color: #fff4ce; color: #664d03; }
    .status-badge.completed { background-color: #d4edda; color: #155724; }

    .param-list { margin: 0; padding-left: 20px; color: #444; font-size: 0.95em; }
    .param-list li { margin-bottom: 4px; }

    .actions-col { text-align: right; }
    
    /* UPDATED: Matches the "View Details" style */
    .view-report-link {
      display: inline-block;
      color: #0078d4;
      font-weight: 500;
      text-decoration: none;
      font-size: 0.9rem;
      padding: 6px 12px;
      border: 1px solid transparent;
      border-radius: 4px;
      transition: all 0.2s;
    }
    .view-report-link:hover {
      background-color: #eff6fc;
      border-color: #c7e0f4;
    }
    
    .no-action { color: #ccc; }

    .loading, .error, .empty-text { padding: 20px; text-align: center; color: #666; }
    .error { color: #d9534f; }
  `]
})
export class RequestsComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private cd = inject(ChangeDetectorRef);
  private router = inject(Router);

  requests: LabRequest[] = [];
  isLoading = false;
  errorMessage = '';
  
  private routerSubscription: Subscription | undefined;

  constructor() {
    this.routerSubscription = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.fetchRequests();
    });
  }

  ngOnInit() {
    this.fetchRequests();
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  fetchRequests() {
    this.isLoading = true;
    
    this.http.get<LabRequestsResponse>('http://localhost:5000/api/LabRequests')
      .subscribe({
        next: (response) => {
          this.requests = response.labRequests;
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error('Error fetching requests:', err);
          this.errorMessage = 'Failed to load lab requests.';
          this.isLoading = false;
          this.cd.detectChanges();
        }
      });
  }
}