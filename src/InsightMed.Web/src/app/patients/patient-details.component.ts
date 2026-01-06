import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';

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
  labRequestState: number; // 0 = Pending, 1 = Completed
  patientId: number;
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
}

@Component({
  selector: 'app-patient-details',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe],
  template: `
    <div class="page-container">
      
      <div class="header">
        <h2>Patient Details</h2>
      </div>

      <div *ngIf="isLoading" class="loading">Loading patient details...</div>
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
          <h3>Lab Requests</h3>
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
                  <td>{{ req.created | date:'MMM d, y, h:mm a' }}</td>
                  <td>
                    <span class="status-badge" 
                          [class.pending]="req.labRequestState === 0"
                          [class.completed]="req.labRequestState === 1">
                      {{ req.labRequestState === 1 ? 'Completed' : 'Pending' }}
                    </span>
                  </td>
                  <td class="actions-col">
                    <a *ngIf="req.labRequestState === 1" 
                       [routerLink]="['/reports', req.id]" 
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
    
    .loading, .error, .empty-text { padding: 20px; text-align: center; color: #666; }
    .error { color: #d9534f; }
  `]
})
export class PatientDetailsComponent implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private cd = inject(ChangeDetectorRef);

  patient: PatientDetails | null = null;
  isLoading = false;
  errorMessage = '';

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    
    if (id) {
      this.fetchPatientDetails(id);
    } else {
      this.errorMessage = 'Invalid Patient ID';
    }
  }

  fetchPatientDetails(id: string) {
    this.isLoading = true;
    this.http.get<PatientDetails>(`http://localhost:5000/api/Patients/${id}`)
      .subscribe({
        next: (data) => {
          this.patient = data;
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = 'Failed to load patient details.';
          this.isLoading = false;
          this.cd.detectChanges();
        }
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