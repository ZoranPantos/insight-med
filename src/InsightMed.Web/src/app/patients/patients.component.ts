import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { LoadingSpinnerComponent } from '../shared/loading-spinner.component'; 

interface Patient {
  id: number;
  uid: string;
  firstName: string;
  lastName: string;
}

interface PatientsResponse {
  patients: Patient[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent],
  template: `
    <div class="page-container">
      <div class="header">
        <h2>Patients</h2>
        <a routerLink="/patients/add" class="create-btn">Add Patient</a>
      </div>

      <app-loading-spinner 
        *ngIf="isLoading" 
        minHeight="200px">
      </app-loading-spinner>

      <div *ngIf="errorMessage" class="error">
        {{ errorMessage }}
      </div>

      <div *ngIf="!isLoading && !errorMessage" class="table-container">
        <table>
          <thead>
            <tr>
              <th>UID</th>
              <th>First Name</th>
              <th>Last Name</th>
              <th class="actions">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let patient of patients">
              <td><span class="uid-badge">{{ patient.uid }}</span></td>
              <td>{{ patient.firstName }}</td>
              <td>{{ patient.lastName }}</td>
              <td class="actions">
                <a [routerLink]="['/patients', patient.id]" class="view-link">View Details</a>
              </td>
            </tr>
            
            <tr *ngIf="patients.length === 0">
              <td colspan="4" class="empty-text">No patients found.</td>
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
    .create-btn:hover { background-color: #005a9e; }
    .create-btn:active { transform: scale(0.98); }

    .table-container { border: 1px solid #e0e0e0; border-radius: 4px; overflow: hidden; }
    table { width: 100%; border-collapse: collapse; background: white; }
    th, td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #f0f0f0; }
    th { background-color: #fafafa; font-weight: 600; color: #555; font-size: 0.9em; text-transform: uppercase; letter-spacing: 0.5px; }
    tr:hover { background-color: #f9f9f9; }
    .uid-badge { background-color: #eef3fc; color: #3b5998; padding: 4px 8px; border-radius: 4px; font-size: 0.85em; font-weight: 500; }
    .actions { text-align: right; }
    .view-link { color: #0078d4; text-decoration: none; font-weight: 500; font-size: 0.9em; padding: 6px 12px; border: 1px solid transparent; border-radius: 4px; transition: all 0.2s; }
    .view-link:hover { background-color: #eff6fc; border-color: #c7e0f4; }
    
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
export class PatientsComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private cd = inject(ChangeDetectorRef);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  patients: Patient[] = [];
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
      
      this.fetchPatients(searchKey, page);
    });
  }

  ngOnDestroy() {
    if (this.querySubscription) {
      this.querySubscription.unsubscribe();
    }
  }

  fetchPatients(searchKey: string, page: number) {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.http.get<PatientsResponse>('http://localhost:5000/api/Patients', {
      params: { 
        searchKey: searchKey,
        pageNumber: page
      }
    })
      .subscribe({
        next: (response) => {
          this.patients = response.patients;
          
          this.totalCount = response.totalCount;
          this.currentPage = response.pageNumber;
          this.totalPages = Math.ceil(this.totalCount / response.pageSize);

          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error('Error fetching patients:', err);
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