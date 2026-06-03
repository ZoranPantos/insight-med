import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute, RouterLink } from '@angular/router'; 
import { Subscription } from 'rxjs';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component'; 
import { ErrorDisplayComponent } from '../../shared/error-display/error-display.component';

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
  templateUrl: './requests.component.html',
  styleUrl: './requests.component.css'
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
    
    this.http.get<LabRequestsResponse>('/api/LabRequests', {
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
