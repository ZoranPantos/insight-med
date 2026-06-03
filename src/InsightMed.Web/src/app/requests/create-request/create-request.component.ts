import { Component, OnInit, inject, HostListener, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { ToastService } from '../../services/toast.service';

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
  templateUrl: './create-request.component.html',
  styleUrl: './create-request.component.css'
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
      params: this.http.get<any>('/api/LabParameters'),
      patients: this.http.get<any>('/api/Patients')
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
  }

  filterPatients() {
    if (!this.patientSearchTerm) {
    this.filteredPatients = this.allPatients;
    this.selectedPatient = null;
    this.selectedPatientId = null;
  } else {
    const term = this.patientSearchTerm.toLowerCase();
    this.filteredPatients = this.allPatients.filter(p => {
      const fullName = `${p.firstName} ${p.lastName}`.toLowerCase();
      return fullName.includes(term) || p.uid.toLowerCase().includes(term);
    });
    
    if (this.selectedPatient && 
        this.patientSearchTerm !== `${this.selectedPatient.firstName} ${this.selectedPatient.lastName}`) {
       this.selectedPatient = null;
       this.selectedPatientId = null;
    }
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

    this.http.post('/api/LabRequests', payload)
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
