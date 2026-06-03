import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { ErrorDisplayComponent } from '../../shared/error-display/error-display.component';

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
  smokingStatus: number;
  exerciseLevel: number;
  dietType: number;
  heightCm: number;
  weightKg: number;
  labReports: LabReport[];
  labRequests: LabRequest[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Component({
  selector: 'app-patient-details',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, LoadingSpinnerComponent, ErrorDisplayComponent],
  templateUrl: './patient-details.component.html',
  styleUrl: './patient-details.component.css'
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
    this.errorMessage = '';
    
    this.http.get<PatientDetails>(`/api/Patients/${id}`, {
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

  onUpdate() {
    if (this.patient) {
      this.router.navigate(['/patients/edit', this.patient.id]);
    }
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

  getSmokingStatusString(value: number): string {
    switch (value) {
      case 0: return 'Never';
      case 1: return 'Former';
      case 2: return 'Current';
      default: return 'Unknown';
    }
  }

  getExerciseLevelString(value: number): string {
    switch (value) {
      case 0: return 'Sedentary';
      case 1: return 'Moderate';
      case 2: return 'Active';
      default: return 'Unknown';
    }
  }

  getDietTypeString(value: number): string {
    const diets = [
      'Regular', 'Vegetarian', 'Vegan', 'Gluten Free', 'Lactose Free', 
      'Low Carb', 'Low Sodium', 'Diabetic', 'Renal', 'Soft', 'Liquid'
    ];
    return diets[value] || 'Unknown';
  }
}
