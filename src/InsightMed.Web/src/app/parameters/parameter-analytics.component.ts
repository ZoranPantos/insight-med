import { Component, OnInit, inject, HostListener, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { LoadingSpinnerComponent } from '../shared/loading-spinner.component';
import { ErrorDisplayComponent } from '../shared/error-display.component';

interface EvaluatedParameter {
  id: number;
  name: string;
}

interface AnalyticsResponse {
  evaluatedLabParameters: EvaluatedParameter[];
}

interface PatientLite {
  id: number;
  uid: string;
  firstName: string;
  lastName: string;
}

@Component({
  selector: 'app-parameter-analytics',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, LoadingSpinnerComponent, ErrorDisplayComponent],
  template: `
    <div class="page-container">
      
      <div class="header">
        <div class="title-group">
          <h2>Parameter Analytics</h2>
          
          <div *ngIf="patient" class="patient-info">
            <span class="patient-name">{{ patient.firstName }} {{ patient.lastName }}</span>
            <span class="uid-badge">{{ patient.uid }}</span>
          </div>
        </div>
        
        <a *ngIf="patientId" [routerLink]="['/patients', patientId]" class="back-link">
          ← Back to Patient Details
        </a>
      </div>

      <app-loading-spinner 
        *ngIf="isLoading" 
        message="Loading data..."
        minHeight="300px">
      </app-loading-spinner>

      <app-error-display
        *ngIf="errorMessage && !isLoading"
        [message]="errorMessage"
        minHeight="300px">
      </app-error-display>

      <div *ngIf="!isLoading && !errorMessage" class="content">
        
        <div class="controls-card">
          <div class="form-group">
            <label>Select Parameter to Analyze</label>
            
            <div class="custom-select" (click)="toggleDropdown($event)" [class.open]="isDropdownOpen">
              
              <div class="selected-value">
                {{ getSelectedParameterName() }}
              </div>
              <span class="arrow">▼</span>

              <div *ngIf="isDropdownOpen" class="dropdown-list" (click)="$event.stopPropagation()">
                
                <div class="dropdown-search-wrapper">
                  <input 
                    type="text" 
                    class="dropdown-search-input" 
                    placeholder="Search parameter..." 
                    [(ngModel)]="searchTerm"
                    (input)="filterParameters()"
                    autofocus
                  />
                </div>

                <div class="options-container">
                  <div *ngFor="let param of filteredParameters" 
                       class="option-item" 
                       (click)="selectParameter(param)">
                    {{ param.name }}
                  </div>
                  
                  <div *ngIf="filteredParameters.length === 0" class="empty-option">
                    No parameters found
                  </div>
                </div>

              </div>
            </div>

          </div>
        </div>

        <div *ngIf="parameters.length === 0" class="empty-state">
          No evaluated parameters found for this patient.
        </div>

        <div *ngIf="selectedParameterId" class="analytics-area">
          <p class="placeholder-text">Analytics for parameter ID: {{ selectedParameterId }}</p>
        </div>

      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; }
    
    .header { 
      display: flex; 
      justify-content: space-between; 
      align-items: center; 
      margin-bottom: 25px; 
      border-bottom: 1px solid #eee;
      padding-bottom: 15px;
    }
    
    .title-group { display: flex; flex-direction: column; gap: 8px; }
    h2 { margin: 0; color: #333; }

    .patient-info { display: flex; align-items: center; gap: 10px; }
    .patient-name { font-size: 1.1rem; color: #666; }
    .uid-badge { 
        background-color: #eef3fc; 
        color: #3b5998; 
        padding: 4px 8px; 
        border-radius: 4px; 
        font-weight: 500; 
        font-size: 0.9em; 
    }

    .back-link {
      color: #0078d4;
      text-decoration: none;
      font-weight: 500;
      font-size: 0.95rem;
      cursor: pointer;
    }
    .back-link:hover { text-decoration: underline; }

    .controls-card {
      background: white;
      padding: 20px;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      margin-bottom: 30px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.02);
    }

    .form-group { max-width: 400px; }
    label { display: block; color: #666; font-size: 0.9em; font-weight: 500; margin-bottom: 8px; margin-left: 5px; }

    .custom-select {
      width: 100%; padding: 10px 20px;
      border: 1px solid #ccc; border-radius: 25px;
      background: white; cursor: pointer; font-size: 0.95rem;
      position: relative; display: flex; justify-content: space-between; align-items: center;
      user-select: none; box-sizing: border-box;
    }
    .custom-select.open { border-color: #0078d4; box-shadow: 0 0 0 3px rgba(0,120,212,0.1); }
    
    .arrow { font-size: 0.8em; color: #666; }

    .dropdown-list {
      position: absolute; top: 105%; left: 0; right: 0;
      background: white; border: 1px solid #ddd; border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15); z-index: 1000;
      overflow: hidden; 
      display: flex; flex-direction: column;
    }

    .dropdown-search-wrapper { 
      padding: 10px; 
      border-bottom: 1px solid #eee; 
      background: #fafafa; 
    }
    .dropdown-search-input {
      width: 100%; padding: 8px 12px; 
      border: 1px solid #ddd; border-radius: 6px; 
      outline: none; box-sizing: border-box;
    }
    .dropdown-search-input:focus { border-color: #0078d4; }

    .options-container {
      max-height: 250px;
      overflow-y: auto;
    }

    .option-item { padding: 10px 20px; color: #333; transition: background 0.1s; cursor: pointer; }
    .option-item:hover { background-color: #f5f5f5; color: #0078d4; }
    
    .empty-option { padding: 15px; text-align: center; color: #999; font-style: italic; }

    .empty-state { text-align: center; color: #777; font-style: italic; padding: 40px; }
    
    .analytics-area { 
      padding: 20px; 
      background: #fafafa; 
      border-radius: 8px; 
      border: 1px dashed #ccc; 
      text-align: center;
    }
    .placeholder-text { color: #999; }
  `]
})
export class ParameterAnalyticsComponent implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private cd = inject(ChangeDetectorRef);

  patientId: string | null = null;
  patient: PatientLite | null = null;
  
  parameters: EvaluatedParameter[] = [];
  filteredParameters: EvaluatedParameter[] = [];
  
  selectedParameterId: number | null = null;
  
  // Dropdown state
  isDropdownOpen = false;
  searchTerm = '';

  isLoading = true;
  errorMessage = '';

  ngOnInit() {
    this.patientId = this.route.snapshot.paramMap.get('id');
    
    if (this.patientId) {
      this.loadData(this.patientId);
    } else {
      this.errorMessage = 'Invalid Patient ID';
      this.isLoading = false;
    }
  }

  loadData(id: string) {
    this.isLoading = true;
    
    forkJoin({
      patient: this.http.get<PatientLite>(`http://localhost:5000/api/Patients/${id}`),
      analytics: this.http.get<AnalyticsResponse>(`http://localhost:5000/api/Patients/${id}/evaluatedParameters`)
    }).subscribe({
      next: (response) => {
        this.patient = response.patient;
        this.parameters = response.analytics.evaluatedLabParameters;
        this.filteredParameters = this.parameters;

        if (this.parameters.length > 0) {
          this.selectedParameterId = this.parameters[0].id;
        }

        this.isLoading = false;
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load analytics data.';
        this.isLoading = false;
        this.cd.detectChanges();
      }
    });
  }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
    if (this.isDropdownOpen) {
      this.searchTerm = '';
      this.filteredParameters = this.parameters;
    }
  }

  filterParameters() {
    if (!this.searchTerm) {
      this.filteredParameters = this.parameters;
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredParameters = this.parameters.filter(p => 
        p.name.toLowerCase().includes(term)
      );
    }
  }

  selectParameter(param: EvaluatedParameter) {
    this.selectedParameterId = param.id;
    this.isDropdownOpen = false;
    this.onParameterChange();
  }

  getSelectedParameterName(): string {
    if (!this.selectedParameterId) return 'Select Parameter';
    const param = this.parameters.find(p => p.id === this.selectedParameterId);
    return param ? param.name : 'Select Parameter';
  }

  @HostListener('document:click')
  closeDropdown() {
    this.isDropdownOpen = false;
  }

  onParameterChange() {
    console.log('Selected Parameter ID:', this.selectedParameterId);
    // TODO: Logic for fetching charts/analytics will go here
  }
}