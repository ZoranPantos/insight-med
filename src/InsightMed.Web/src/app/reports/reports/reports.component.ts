import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink, Router } from '@angular/router'; 
import { Subscription } from 'rxjs';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component'; 
import { ErrorDisplayComponent } from '../../shared/error-display/error-display.component';

interface LabReport {
  id: number;
  created: string;
  patientFullName: string;
  patientUid: string;
}

interface LabReportsResponse {
  labReports: LabReport[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink, LoadingSpinnerComponent, ErrorDisplayComponent], 
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.css'
})
export class ReportsComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private cd = inject(ChangeDetectorRef);
  private route = inject(ActivatedRoute); 
  private router = inject(Router);

  reports: LabReport[] = [];
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
      
      this.fetchReports(searchKey, page);
    });
  }

  ngOnDestroy() {
    if (this.querySubscription) {
      this.querySubscription.unsubscribe();
    }
  }

  fetchReports(searchKey: string, page: number) {
    this.isLoading = true;
    this.errorMessage = ''; 
    
    this.http.get<LabReportsResponse>('/api/LabReports', {
      params: { 
        searchKey: searchKey,
        pageNumber: page
      } 
    })
      .subscribe({
        next: (response) => {
          this.reports = response.labReports;
          this.totalCount = response.totalCount;
          this.currentPage = response.pageNumber;
          this.totalPages = Math.ceil(this.totalCount / response.pageSize);

          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error('Error fetching reports:', err);
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
