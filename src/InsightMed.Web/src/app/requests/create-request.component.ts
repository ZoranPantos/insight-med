import { Component, OnInit, inject, HostListener, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { LoadingSpinnerComponent } from '../shared/loading-spinner.component';
import { ToastService } from '../services/toast.service';

interface LabParameter {
  id: number;
  name: string;
}

interface Patient {
  id: number;
  uid: string;
  firstName: string;
  lastName: string;
}

@Component({
  selector: 'app-create-request',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  template: `
    <div class="page-container">
      
      <div class="header">
        <h2>Create Lab Request</h2>
      </div>

      <app-loading-spinner 
        *ngIf="isLoadingData || isSubmitting" 
        [message]="isSubmitting ? 'Creating request...' : 'Loading data...'"
        minHeight="300px">
      </app-loading-spinner>

      <div class="form-container" *ngIf="!isLoadingData && !isSubmitting">
        
        <div class="form-content">
          
          <div class="form-group">
            <label>Select Patient</label>
            <div class="searchable-dropdown" (click)="$event.stopPropagation()">
              
              <input 
                type="text" 
                class="pill-input search-input" 
                placeholder="Search by Name or UID..."
                [(ngModel)]="patientSearchTerm" 
                (focus)="openPatientDropdown()"
                (input)="filterPatients()"
              />
              <span class="icon">🔍</span>

              <div *ngIf="isPatientDropdownOpen" class="dropdown-list">
                <div *ngFor="let p of filteredPatients" 
                     class="option-item" 
                     (click)="selectPatient(p)">
                  <span class="p-name">{{ p.firstName }} {{ p.lastName }}</span>
                  <span class="p-uid">{{ p.uid }}</span>
                </div>
                <div *ngIf="filteredPatients.length === 0" class="empty-option">
                  No patients found
                </div>
              </div>
            </div>
            
            <div *ngIf="selectedPatient" class="selected-badge">
              Selected: <strong>{{ selectedPatient.firstName }} {{ selectedPatient.lastName }}</strong> ({{ selectedPatient.uid }})
            </div>
          </div>

          <div class="form-group">
            <label>Select Parameters (at least one)</label>
            
            <div class="multi-select-box" (click)="toggleParamDropdown($event)">
              
              <div *ngIf="selectedParams.length === 0" class="placeholder-text">
                Click to select parameters...
              </div>

              <div *ngFor="let p of selectedParams" class="chip" (click)="$event.stopPropagation()">
                {{ p.name }}
                <span class="remove-chip" (click)="removeParam(p)">×</span>
              </div>

              <span class="arrow-icon">▼</span>
            </div>

            <div *ngIf="isParamDropdownOpen" class="dropdown-list param-dropdown" (click)="$event.stopPropagation()">
              
              <div class="dropdown-search-wrapper">
                <input 
                  type="text" 
                  class="dropdown-search-input" 
                  placeholder="Search parameters..." 
                  [(ngModel)]="paramSearchTerm"
                  (input)="filterParams()"
                  autofocus
                />
              </div>

              <div class="checkbox-list">
                <label *ngFor="let param of filteredParams" class="checkbox-item">
                  <input 
                    type="checkbox" 
                    [checked]="isParamSelected(param.id)"
                    (change)="toggleParamSelection(param)"
                  />
                  <span class="checkmark"></span>
                  <span class="label-text">{{ param.name }}</span>
                </label>
                
                <div *ngIf="filteredParams.length === 0" class="empty-option">
                  No parameters found
                </div>
              </div>
            </div>

          </div>

          <div *ngIf="errorMessages.length > 0" class="message error-message">
            <ul>
              <li *ngFor="let msg of errorMessages">{{ msg }}</li>
            </ul>
          </div>

          <div class="actions">
            <button class="cancel-btn" (click)="onCancel()">Cancel</button>
            <button class="submit-btn" (click)="onSubmit()" [disabled]="isSubmitting">
              Send
            </button>
          </div>

        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; max-width: 800px; margin: 0 auto; }
    .header { margin-bottom: 25px; border-bottom: 1px solid #eee; padding-bottom: 15px; }
    h2 { margin: 0; color: #333; }
    
    .form-container { display: flex; flex-direction: column; }
    .form-content { display: flex; flex-direction: column; gap: 20px; } 
    
    .form-group { display: flex; flex-direction: column; gap: 8px; position: relative; }
    
    label { font-size: 0.9em; font-weight: 500; color: #666; margin-left: 5px; }

    .dropdown-list {
      position: absolute; 
      top: 110%; 
      left: 0; 
      right: 0;
      background: white; 
      border: 1px solid #ddd; 
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15); 
      z-index: 1000;
      display: flex;
      flex-direction: column;
      max-height: 250px; 
      overflow-y: auto;
    }

    .dropdown-list.param-dropdown {
      overflow-y: hidden;
    }

    .empty-option { padding: 15px; text-align: center; color: #999; }

    .searchable-dropdown { position: relative; width: 100%; }
    
    .pill-input {
      width: 100%; padding: 12px 20px; padding-right: 40px;
      border: 1px solid #ccc; border-radius: 25px; 
      outline: none; font-size: 0.95rem; box-sizing: border-box; 
      transition: border-color 0.2s, box-shadow 0.2s;
    }
    .pill-input:focus { border-color: #0078d4; box-shadow: 0 0 0 3px rgba(0,120,212,0.15); }

    .icon { position: absolute; right: 15px; top: 50%; transform: translateY(-50%); color: #999; pointer-events: none; }

    .option-item {
      padding: 10px 20px; cursor: pointer; display: flex; justify-content: space-between;
      border-bottom: 1px solid #f9f9f9;
      min-height: 40px; align-items: center;
    }
    .option-item:last-child { border-bottom: none; }
    .option-item:hover { background-color: #f0f8ff; color: #0078d4; }
    
    .p-name { font-weight: 500; }
    .p-uid { color: #888; font-size: 0.85em; background: #eee; padding: 2px 6px; border-radius: 4px; }

    .selected-badge {
      background-color: #eef3fc; color: #333; padding: 10px 15px;
      border-radius: 8px; font-size: 0.9em; border: 1px solid #c7e0f4; margin-top: 5px;
    }

    .multi-select-box {
      border: 1px solid #ccc; border-radius: 25px;
      min-height: 45px; padding: 8px 15px;
      display: flex; flex-wrap: wrap; gap: 8px; align-items: center;
      cursor: pointer; background: white; position: relative;
    }
    .multi-select-box:hover { border-color: #999; }
    
    .placeholder-text { color: #777; font-size: 0.95rem; margin-left: 5px; }
    .arrow-icon { margin-left: auto; color: #666; font-size: 0.8em; }

    .chip {
      background-color: #eef3fc; color: #0078d4; border: 1px solid #c7e0f4;
      border-radius: 16px; padding: 4px 10px; font-size: 0.9em; font-weight: 500;
      display: flex; align-items: center; gap: 6px;
    }
    .remove-chip {
      cursor: pointer; font-size: 1.1em; line-height: 1; color: #005a9e;
    }
    .remove-chip:hover { color: #d9534f; }

    .dropdown-search-wrapper { 
      padding: 10px; 
      border-bottom: 1px solid #eee; 
      background: #fafafa;
      flex-shrink: 0;
    }
    .dropdown-search-input {
      width: 100%; padding: 8px 12px; border: 1px solid #ddd; border-radius: 6px; outline: none;
    }
    .dropdown-search-input:focus { border-color: #0078d4; }

    .checkbox-list { 
      flex: 1;
      display: flex;
      flex-direction: column;
      overflow-y: auto; 
    }

    .checkbox-item {
      display: flex; align-items: center; cursor: pointer; user-select: none;
      padding: 10px 20px; transition: background 0.1s; border-bottom: 1px solid #f9f9f9;
      min-height: 40px;
    }
    .checkbox-item:hover { background-color: #f5f5f5; }

    .checkbox-item input { display: none; } 
    .checkmark {
      width: 18px; height: 18px; border: 2px solid #ccc; border-radius: 4px;
      margin-right: 10px; position: relative; transition: all 0.2s;
    }
    .checkbox-item input:checked + .checkmark {
      background-color: #0078d4; border-color: #0078d4;
    }
    .checkbox-item input:checked + .checkmark::after {
      content: ''; position: absolute; left: 5px; top: 1px;
      width: 4px; height: 10px; border: solid white;
      border-width: 0 2px 2px 0; transform: rotate(45deg);
    }
    .checkbox-item input:checked ~ .label-text { color: #0078d4; font-weight: 600; }

    .message { padding: 10px 15px; border-radius: 8px; margin-top: 10px; }
    .error-message { background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
    ul { margin: 0; padding-left: 15px; }
    li { margin-bottom: 3px; }

    .actions { display: flex; justify-content: flex-end; align-items: center; gap: 15px; margin-top: 20px; }

    button {
      padding: 10px 24px; border: none; border-radius: 25px; 
      cursor: pointer; font-weight: 600; font-size: 0.95rem;
      transition: background-color 0.2s, transform 0.1s; min-width: 120px;
    }
    button:active { transform: scale(0.98); }

    .cancel-btn { background-color: #e0e0e0; color: #333; }
    .cancel-btn:hover { background-color: #d0d0d0; }

    .submit-btn { background-color: #0078d4; color: white; }
    .submit-btn:hover:not(:disabled) { background-color: #005a9e; }
    .submit-btn:disabled { background-color: #a0cce8; cursor: not-allowed; }
  `]
})
export class CreateRequestComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);
  private cd = inject(ChangeDetectorRef);
  private toastService = inject(ToastService);

  allPatients: Patient[] = [];
  filteredPatients: Patient[] = [];
  
  allLabParameters: LabParameter[] = [];
  filteredParams: LabParameter[] = [];

  patientSearchTerm = '';
  selectedPatient: Patient | null = null;
  selectedPatientId: number | null = null;

  selectedParams: LabParameter[] = [];
  paramSearchTerm = '';

  isLoadingData = true;
  isSubmitting = false;
  isPatientDropdownOpen = false;
  isParamDropdownOpen = false;
  errorMessages: string[] = [];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoadingData = true; 
    forkJoin({
      params: this.http.get<any>('http://localhost:5000/api/LabParameters'),
      patients: this.http.get<any>('http://localhost:5000/api/Patients')
    }).subscribe({
      next: (response) => {
        this.allLabParameters = response.params.labParameters;
        this.filteredParams = this.allLabParameters;

        this.allPatients = response.patients.patients;
        this.filteredPatients = this.allPatients; 
        
        this.isLoadingData = false;
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error('Error loading initial data', err);
        this.toastService.show('Failed to load form data', 'error');
        this.errorMessages = ['Failed to load data'];
        this.isLoadingData = false;
        this.cd.detectChanges();
      }
    });
  }

  openPatientDropdown() {
    this.isPatientDropdownOpen = true;
    this.isParamDropdownOpen = false;
    this.filterPatients(); 
  }

  filterPatients() {
    this.selectedPatientId = null;
    this.selectedPatient = null;

    if (!this.patientSearchTerm) {
      this.filteredPatients = this.allPatients;
    } else {
      const term = this.patientSearchTerm.toLowerCase();
      this.filteredPatients = this.allPatients.filter(p => {
        const fullName = `${p.firstName} ${p.lastName}`.toLowerCase();
        return fullName.includes(term) || p.uid.toLowerCase().includes(term);
      });
    }
    this.isPatientDropdownOpen = true;
  }

  selectPatient(patient: Patient) {
    this.selectedPatient = patient;
    this.selectedPatientId = patient.id;
    this.patientSearchTerm = `${patient.firstName} ${patient.lastName}`; 
    this.isPatientDropdownOpen = false;
  }

  toggleParamDropdown(event: Event) {
    event.stopPropagation();
    this.isParamDropdownOpen = !this.isParamDropdownOpen;
    this.isPatientDropdownOpen = false;
    
    if (this.isParamDropdownOpen) {
      this.paramSearchTerm = '';
      this.filteredParams = this.allLabParameters;
    }
  }

  filterParams() {
    if (!this.paramSearchTerm) {
      this.filteredParams = this.allLabParameters;
    } else {
      const term = this.paramSearchTerm.toLowerCase();
      this.filteredParams = this.allLabParameters.filter(p => 
        p.name.toLowerCase().includes(term)
      );
    }
  }

  toggleParamSelection(param: LabParameter) {
    const index = this.selectedParams.findIndex(p => p.id === param.id);
    if (index === -1) {
      this.selectedParams.push(param);
    } else {
      this.selectedParams.splice(index, 1);
    }
  }

  removeParam(param: LabParameter) {
    const index = this.selectedParams.findIndex(p => p.id === param.id);
    if (index !== -1) {
      this.selectedParams.splice(index, 1);
    }
  }

  isParamSelected(id: number): boolean {
    return this.selectedParams.some(p => p.id === id);
  }

  @HostListener('document:click')
  closeDropdowns() {
    this.isPatientDropdownOpen = false;
    this.isParamDropdownOpen = false;
  }

  onCancel() {
    this.router.navigate(['/requests']);
  }

  onSubmit() {
    if (!this.selectedPatientId || this.selectedParams.length === 0) {
      this.errorMessages = ['Please select a patient and at least one parameter'];
      return;
    }

    this.isSubmitting = true;
    this.errorMessages = [];

    const payload = {
      patientId: this.selectedPatientId,
      labParameterIds: this.selectedParams.map(p => p.id)
    };

    this.http.post('http://localhost:5000/api/LabRequests', payload)
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.toastService.show('Action successful', 'success');
          this.router.navigate(['/requests']);
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.isSubmitting = false;
          
          this.toastService.show('Action failed', 'error');

          if (err.error && err.error.detail) {
            const rawMessages = err.error.detail.split(',');
            
            this.errorMessages = rawMessages.map((msg: string) => 
              msg.trim()
                 .replace(/^:\s*/, '')
                 .replace(/\.$/, '')
            );
          } else {
            this.errorMessages = ['Failed to create request'];
          }
          this.cd.detectChanges();
        }
      });
  }
}