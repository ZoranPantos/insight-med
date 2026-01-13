import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { LoadingSpinnerComponent } from '../shared/loading-spinner.component';

interface LabReport {
  id: number;
  content: string;
  created: string;
  labRequestId: number;
  patientId: number;
}

interface LabRequest {
  id: number;
  created: string;
  labRequestState: number; 
  patientId: number;
  labReportId: number | null; 
}

interface PatientDetails {
  id: number;
  uid: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  gender: number;      
  bloodGroup: number;  
  labReports: LabReport[];
  labRequests: LabRequest[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Component({
  selector: 'app-patient-details',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, LoadingSpinnerComponent],
  template: `
    <div class="page-container">
      
      <div class="header">
        <h2>Patient Details</h2>
      </div>

      <app-loading-spinner 
        *ngIf="isLoading" 
        minHeight="300px">
      </app-loading-spinner>

      <div *ngIf="errorMessage" class="error">{{ errorMessage }}</div>

      <div *ngIf="!isLoading && patient" class="content-wrapper">
        
        <div class="info-card">
          <div class="info-header">
            <h3>{{ patient.firstName }} {{ patient.lastName }}</h3>
            <span class="uid-badge">{{ patient.uid }}</span>
          </div>
          
          <div class="info-grid">
            <div class="info-item">
              <label>Email</label>
              <div>{{ patient.email }}</div>
            </div>
            <div class="info-item">
              <label>Phone</label>
              <div>{{ patient.phone }}</div>
            </div>
            <div class="info-item">
              <label>Date of Birth</label>
              <div>{{ patient.dateOfBirth | date:'mediumDate' }}</div>
            </div>
            <div class="info-item">
              <label>Gender</label>
              <div>{{ getGenderString(patient.gender) }}</div>
            </div>
            <div class="info-item">
              <label>Blood Group</label>
              <div>{{ getBloodGroupString(patient.bloodGroup) }}</div>
            </div>
          </div>
        </div>

        <div class="section">
          <h3>Completed Lab Requests</h3>
          <div class="table-container">
            <table>
              <thead>
                <tr>
                  <th>Date Created</th>
                  <th>Status</th>
                  <th class="actions-col">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let req of patient.labRequests">
                  <td class="date-col">{{ req.created | date:'MMM d, y, h:mm a' }}</td>
                  <td>
                    <span class="status-badge" 
                          [class.pending]="req.labRequestState === 0"
                          [class.completed]="req.labRequestState === 1">
                      {{ req.labRequestState === 1 ? 'Completed' : 'Pending' }}
                    </span>
                  </td>
                  <td class="actions-col">
                    <a *ngIf="req.labRequestState === 1 && req.labReportId" 
                       [routerLink]="['/reports', req.labReportId]" 
                       class="view-link">
                       View Report
                    </a>
                  </td>
                </tr>
                <tr *ngIf="patient.labRequests.length === 0">
                  <td colspan="3" class="empty-text">No requests on file.</td>
                </tr>
              </tbody>
            </table>
          </div>

          <div *ngIf="totalPages > 1" class="pagination-controls">
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

      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; }
    
    .header { margin-bottom: 25px; }
    h2 { margin: 0; color: #333; }
    h3 { margin: 0 0 15px 0; color: #444; font-size: 1.1rem; }

    .info-card { background: white; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px; margin-bottom: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.02); }
    .info-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #eee; padding-bottom: 15px; margin-bottom: 15px; }
    .info-header h3 { margin: 0; font-size: 1.3rem; color: #0078d4; }
    .uid-badge { background-color: #eef3fc; color: #3b5998; padding: 4px 8px; border-radius: 4px; font-weight: 500; font-size: 0.9em; }

    .info-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 20px; }
    .info-item label { display: block; color: #888; font-size: 0.85em; margin-bottom: 4px; font-weight: 500; }
    .info-item div { color: #333; font-weight: 500; }

    .section { margin-bottom: 30px; }
    .table-container { border: 1px solid #e0e0e0; border-radius: 4px; overflow: hidden; }
    table { width: 100%; border-collapse: collapse; background: white; }
    th, td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #f0f0f0; }
    th { background-color: #fafafa; font-weight: 600; color: #555; font-size: 0.9em; text-transform: uppercase; }
    tr:hover { background-color: #f9f9f9; }

    .date-col { white-space: nowrap; color: #666; font-size: 0.9rem; }

    .status-badge { display: inline-block; padding: 4px 10px; border-radius: 12px; font-size: 0.85em; font-weight: 500; }
    .status-badge.pending { background-color: #fff4ce; color: #664d03; }
    .status-badge.completed { background-color: #d4edda; color: #155724; }

    .actions-col { text-align: right; }

    .view-link { 
      color: #0078d4; 
      text-decoration: none; 
      font-weight: 500; 
      font-size: 0.9em; 
      padding: 6px 12px; 
      border: 1px solid transparent; 
      border-radius: 4px; 
      transition: all 0.2s; 
    }
    .view-link:hover { 
      background-color: #eff6fc; 
      border-color: #c7e0f4; 
    }
    
    /* Removed .loading style */
    .error, .empty-text { padding: 20px; text-align: center; color: #666; }
    .error { color: #d9534f; }

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
export class PatientDetailsComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cd = inject(ChangeDetectorRef);

  patient: PatientDetails | null = null;
  isLoading = false;
  errorMessage = '';

  currentPage = 1;
  totalPages = 1;
  
  private querySubscription: Subscription | undefined;

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    
    if (id) {
      this.querySubscription = this.route.queryParams.subscribe(params => {
        const page = params['pageNumber'] ? Number(params['pageNumber']) : 1;
        this.fetchPatientDetails(id, page);
      });
    } else {
      this.errorMessage = 'Invalid Patient ID';
    }
  }

  ngOnDestroy() {
    if (this.querySubscription) {
      this.querySubscription.unsubscribe();
    }
  }

  fetchPatientDetails(id: string, page: number) {
    this.isLoading = true;
    
    this.http.get<PatientDetails>(`http://localhost:5000/api/Patients/${id}`, {
      params: { pageNumber: page }
    }).subscribe({
        next: (data) => {
          this.patient = data;
          
          this.currentPage = data.pageNumber;
          this.totalPages = Math.ceil(data.totalCount / data.pageSize);

          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
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

  getGenderString(value: number): string {
    switch (value) {
      case 0: return 'Male';
      case 1: return 'Female';
      default: return 'Unknown';
    }
  }

  getBloodGroupString(value: number): string {
    switch (value) {
      case 0: return 'A Positive';
      case 1: return 'A Negative';
      case 2: return 'B Positive';
      case 3: return 'B Negative';
      case 4: return 'AB Positive';
      case 5: return 'AB Negative';
      case 6: return 'O Positive';
      case 7: return 'O Negative';
      default: return 'Unknown';
    }
  }
}