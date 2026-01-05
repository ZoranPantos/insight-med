import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterLink, Router, NavigationEnd } from '@angular/router';
import { Subscription, filter } from 'rxjs';

interface Patient {
  id: number;
  uid: string;
  firstName: string;
  lastName: string;
}

interface PatientsResponse {
  patients: Patient[];
}

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="header">
        <h2>Patients</h2>
      </div>

      <div *ngIf="isLoading" class="loading">
        Loading patients...
      </div>

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
    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; }
    .header { margin-bottom: 20px; }
    h2 { margin: 0; color: #333; }
    .table-container { border: 1px solid #e0e0e0; border-radius: 4px; overflow: hidden; }
    table { width: 100%; border-collapse: collapse; background: white; }
    th, td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #f0f0f0; }
    th { background-color: #fafafa; font-weight: 600; color: #555; font-size: 0.9em; text-transform: uppercase; letter-spacing: 0.5px; }
    tr:hover { background-color: #f9f9f9; }
    .uid-badge { background-color: #eef3fc; color: #3b5998; padding: 4px 8px; border-radius: 4px; font-size: 0.85em; font-weight: 500; }
    .actions { text-align: right; }
    .view-link { color: #0078d4; text-decoration: none; font-weight: 500; font-size: 0.9em; padding: 6px 12px; border: 1px solid transparent; border-radius: 4px; transition: all 0.2s; }
    .view-link:hover { background-color: #eff6fc; border-color: #c7e0f4; }
    .loading, .error, .empty-text { padding: 20px; text-align: center; color: #666; }
    .error { color: #d9534f; }
  `]
})
export class PatientsComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private cd = inject(ChangeDetectorRef);
  private router = inject(Router);

  patients: Patient[] = [];
  isLoading = false;
  errorMessage = '';
  private routerSubscription: Subscription | undefined;

  constructor() {
    this.routerSubscription = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.fetchPatients();
    });
  }

  ngOnInit() {
    this.fetchPatients();
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  fetchPatients() {
    this.isLoading = true;
    this.http.get<PatientsResponse>('http://localhost:5000/api/Patients')
      .subscribe({
        next: (response) => {
          this.patients = response.patients;
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error('Error fetching patients:', err);
          this.errorMessage = 'Failed to load patients. Please try again later.';
          this.isLoading = false;
          this.cd.detectChanges();
        }
      });
  }
}