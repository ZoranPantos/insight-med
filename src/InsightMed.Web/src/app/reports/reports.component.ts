import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, NavigationEnd, RouterLink } from '@angular/router'; 
import { Subscription, filter } from 'rxjs';

interface LabReport {
  id: number;
  created: string;
  patientFullName: string;
  patientUid: string;
}

interface LabReportsResponse {
  labReports: LabReport[];
}

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink], 
  template: `
    <div class="page-container">
      <div class="header">
        <h2>Lab Reports</h2>
      </div>

      <div *ngIf="isLoading" class="loading">
        Loading reports...
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
              <th class="actions">Actions</th> 
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let report of reports">
              <td class="date-col">
                {{ report.created | date:'MMM d, y, h:mm a' }}
              </td>

              <td>
                <div class="patient-name">{{ report.patientFullName }}</div>
                <div class="patient-uid">{{ report.patientUid }}</div>
              </td>

              <td class="actions">
                <a [routerLink]="['/reports', report.id]" class="view-link">
                   View Details
                </a>
              </td>
            </tr>

            <tr *ngIf="reports.length === 0">
              <td colspan="3" class="empty-text">No reports found.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; }
    .header { margin-bottom: 20px; }
    h2 { margin: 0; color: #333; }

    .table-container { border: 1px solid #e0e0e0; border-radius: 4px; overflow: hidden; }
    table { width: 100%; border-collapse: collapse; background: white; }
    th, td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #f0f0f0; vertical-align: middle; }
    
    th {
      background-color: #fafafa; font-weight: 600; color: #555;
      font-size: 0.9em; text-transform: uppercase; letter-spacing: 0.5px;
    }
    tr:hover { background-color: #f9f9f9; }

    .date-col { white-space: nowrap; color: #666; font-size: 0.9rem; }
    .patient-name { font-weight: 500; }
    .patient-uid { font-size: 0.8em; color: #888; margin-top: 2px; }

    .actions { text-align: right; }
    
    .view-link {
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
    .view-link:hover {
      background-color: #eff6fc;
      border-color: #c7e0f4;
    }
    
    .loading, .error, .empty-text { padding: 20px; text-align: center; color: #666; }
    .error { color: #d9534f; }
  `]
})
export class ReportsComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private cd = inject(ChangeDetectorRef);
  private router = inject(Router);

  reports: LabReport[] = [];
  isLoading = false;
  errorMessage = '';
  
  private routerSubscription: Subscription | undefined;

  constructor() {
    this.routerSubscription = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.fetchReports();
    });
  }

  ngOnInit() {
    this.fetchReports();
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  fetchReports() {
    this.isLoading = true;
    
    this.http.get<LabReportsResponse>('http://localhost:5000/api/LabReports')
      .subscribe({
        next: (response) => {
          this.reports = response.labReports;
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error('Error fetching reports:', err);
          this.errorMessage = 'Failed to load lab reports.';
          this.isLoading = false;
          this.cd.detectChanges();
        }
      });
  }
}