import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute, RouterLink } from '@angular/router'; 
import { Subscription } from 'rxjs';
import { LoadingSpinnerComponent } from '../shared/loading-spinner.component'; 
import { ErrorDisplayComponent } from '../shared/error-display.component';

interface LabParameter {
  name: string;
}

interface LabRequest {
  id: number;
  created: string;
  labRequestState: number;
  patientFullName: string;
  patientUid: string;
  labReportId: number | null; 
  labParameters: LabParameter[];
}

interface LabRequestsResponse {
  labRequests: LabRequest[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Component({
  selector: 'app-requests',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink, LoadingSpinnerComponent, ErrorDisplayComponent], 
  template: `
    <div class="page-container">
      <div class="header">
        <h2>Lab Requests</h2>
        <a routerLink="/requests/create" class="create-btn">Create Request</a>
      </div>

      <app-loading-spinner 
        *ngIf="isLoading" 
        minHeight="200px">
      </app-loading-spinner>

      <app-error-display
        *ngIf="errorMessage && !isLoading"
        [message]="errorMessage"
        minHeight="200px">
      </app-error-display>

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
                <a *ngIf="req.labRequestState === 1 && req.labReportId" 
                   [routerLink]="['/reports', req.labReportId]" 
                   class="view-report-link">
                   View Report
                </a>
              </td>
            </tr>

            <tr *ngIf="requests.length === 0">
              <td colspan="5" class="empty-text">No requests found.</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div *ngIf="!isLoading && !errorMessage && totalPages > 1" class="pagination-controls">
        <button 
          class="page-btn" 
          [disabled]="currentPage === 1" 
          (click)="changePage(currentPage - 1)">
          Prev
        </button>

        <span class="page-info">
          {{ currentPage }} / {{ totalPages }}
        </span>

        <button 
          class="page-btn" 
          [disabled]="currentPage === totalPages" 
          (click)="changePage(currentPage + 1)">
          Next
        </button>
      </div>

    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; }
    
    .header { 
      display: flex; 
      justify-content: space-between; 
      align-items: center; 
      margin-bottom: 20px; 
    }
    
    h2 { margin: 0; color: #333; }

    .create-btn {
      padding: 8px 20px;
      background-color: #0078d4; 
      color: white;
      text-decoration: none;
      border: none;
      border-radius: 20px;
      cursor: pointer;
      font-weight: 600; 
      font-size: 0.95rem;
      transition: background-color 0.2s, transform 0.1s;
    }
    .create-btn:hover {
      background-color: #005a9e; 
    }
    .create-btn:active {
      transform: scale(0.98);
    }

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
    
    .empty-text { padding: 20px; text-align: center; color: #666; }

    .pagination-controls {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 15px;
      margin-top: 25px;
    }
    
    .page-info { 
      font-weight: 600; 
      color: #555; 
      font-size: 1rem;
      min-width: 60px;
      text-align: center;
    }

    .page-btn {
      padding: 10px 24px;       
      background-color: #e0e0e0; 
      color: #333;
      border: none;
      border-radius: 20px;     
      cursor: pointer;
      
      font-family: inherit; 
      font-weight: 600;
      font-size: 0.95rem;      
      
      transition: background-color 0.2s, transform 0.1s;
      min-width: 100px;         
    }

    .page-btn:hover:not(:disabled) {
      background-color: #d0d0d0;
    }

    .page-btn:active {
      transform: scale(0.98);
    }

    .page-btn:disabled {
      background-color: #f5f5f5;
      color: #ccc;
      cursor: not-allowed;
      transform: none;
    }
  `]
})
export class RequestsComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private cd = inject(ChangeDetectorRef);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  requests: LabRequest[] = [];
  isLoading = false;
  errorMessage = '';
  
  currentPage = 1;
  totalPages = 1;
  totalCount = 0;
  
  private querySubscription: Subscription | undefined;

  ngOnInit() {
    this.querySubscription = this.route.queryParams.subscribe(params => {
      const searchKey = params['searchKey'] || '';
      const page = params['pageNumber'] ? Number(params['pageNumber']) : 1;
      
      this.fetchRequests(searchKey, page);
    });
  }

  ngOnDestroy() {
    if (this.querySubscription) {
      this.querySubscription.unsubscribe();
    }
  }

  fetchRequests(searchKey: string, page: number) {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.http.get<LabRequestsResponse>('http://localhost:5000/api/LabRequests', {
      params: { 
        searchKey: searchKey,
        pageNumber: page
      }
    })
      .subscribe({
        next: (response) => {
          this.requests = response.labRequests;
          
          this.totalCount = response.totalCount;
          this.currentPage = response.pageNumber;
          this.totalPages = Math.ceil(this.totalCount / response.pageSize);

          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error('Error fetching requests:', err);
          this.errorMessage = 'Failed to load data';
          this.isLoading = false;
          this.cd.detectChanges();
        }
      });
  }

  changePage(newPage: number) {
    if (newPage < 1 || newPage > this.totalPages) return;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { pageNumber: newPage },
      queryParamsHandling: 'merge' 
    });
  }
}