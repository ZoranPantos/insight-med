import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component'; 
import { ErrorDisplayComponent } from '../../shared/error-display/error-display.component';

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
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent, ErrorDisplayComponent],
  templateUrl: './patients.component.html',
  styleUrl: './patients.component.css'
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
    
    this.http.get<PatientsResponse>('/api/Patients', {
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
