import { Component, inject, HostListener, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingSpinnerComponent } from '../shared/loading-spinner.component';
import { ToastService } from '../services/toast.service';

@Component({
  selector: 'app-update-patient',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  template: `
    <div class="page-container">
      <div class="header">
        <h2>Update Patient</h2>
      </div>

      <app-loading-spinner 
        *ngIf="isLoading" 
        minHeight="300px">
      </app-loading-spinner>

      <div class="form-container" *ngIf="!isLoading">
        
        <div class="info-banner">
          <p><strong>Patient:</strong> {{ firstName }} {{ lastName }} ({{ uid }})</p>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Height (cm)</label>
            <input type="number" [(ngModel)]="heightCm" (input)="clearMessages()" class="pill-input" placeholder="e.g. 175" />
          </div>
          <div class="form-group">
            <label>Weight (kg)</label>
            <input type="number" [(ngModel)]="weightKg" (input)="clearMessages()" class="pill-input" placeholder="e.g. 70.5" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Smoking Status</label>
            <div class="custom-select" (click)="toggleSmoking($event)" [class.open]="isSmokingOpen">
              <div class="selected-value">{{ getOptionLabel(smokingOptions, smokingStatus) }}</div>
              <span class="arrow">▼</span>
              <div *ngIf="isSmokingOpen" class="dropdown-list">
                <div *ngFor="let opt of smokingOptions" class="option-item" (click)="selectOption('smokingStatus', opt.value, $event)">
                  {{ opt.label }}
                </div>
              </div>
            </div>
          </div>

          <div class="form-group">
            <label>Exercise Level</label>
            <div class="custom-select" (click)="toggleExercise($event)" [class.open]="isExerciseOpen">
              <div class="selected-value">{{ getOptionLabel(exerciseOptions, exerciseLevel) }}</div>
              <span class="arrow">▼</span>
              <div *ngIf="isExerciseOpen" class="dropdown-list">
                <div *ngFor="let opt of exerciseOptions" class="option-item" (click)="selectOption('exerciseLevel', opt.value, $event)">
                  {{ opt.label }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Diet Type</label>
            <div class="custom-select" (click)="toggleDiet($event)" [class.open]="isDietOpen">
              <div class="selected-value">{{ getOptionLabel(dietOptions, dietType) }}</div>
              <span class="arrow">▼</span>
              <div *ngIf="isDietOpen" class="dropdown-list">
                <div *ngFor="let opt of dietOptions" class="option-item" (click)="selectOption('dietType', opt.value, $event)">
                  {{ opt.label }}
                </div>
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
          <button class="submit-btn" (click)="onSubmit()" [disabled]="isSaving">Update</button>
        </div>

      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; max-width: 800px; margin: 0 auto; }
    .header { margin-bottom: 25px; border-bottom: 1px solid #eee; padding-bottom: 15px; }
    h2 { margin: 0; color: #333; }

    .info-banner { background: #eef3fc; padding: 15px; border-radius: 8px; margin-bottom: 20px; color: #333; }
    .info-banner p { margin: 0; }

    .form-container { display: flex; flex-direction: column; gap: 20px; }
    .form-row { display: flex; gap: 20px; }
    .form-group { flex: 1; display: flex; flex-direction: column; gap: 8px; position: relative; }

    label { font-size: 0.9em; font-weight: 500; color: #666; margin-left: 5px; }

    .pill-input {
      width: 100%; padding: 10px 20px; 
      border: 1px solid #ccc; border-radius: 25px; 
      outline: none; font-size: 0.95rem; box-sizing: border-box; 
      transition: border-color 0.2s; background: white;
    }
    .pill-input:focus { border-color: #0078d4; box-shadow: 0 0 0 3px rgba(0,120,212,0.1); }

    .custom-select {
      width: 100%; padding: 10px 20px;
      border: 1px solid #ccc; border-radius: 25px;
      background: white; cursor: pointer; font-size: 0.95rem;
      position: relative; display: flex; justify-content: space-between; align-items: center;
      user-select: none; box-sizing: border-box;
    }
    .custom-select.open { border-color: #0078d4; box-shadow: 0 0 0 3px rgba(0,120,212,0.1); }
    
    .selected-value.placeholder { color: #888; }
    .arrow { font-size: 0.8em; color: #666; }

    .dropdown-list {
      position: absolute; top: 105%; left: 0; right: 0;
      background: white; border: 1px solid #ddd; border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15); z-index: 1000;
      overflow: hidden; max-height: 200px; overflow-y: auto;
    }

    .option-item { padding: 10px 20px; color: #333; transition: background 0.1s; }
    .option-item:hover { background-color: #f5f5f5; color: #0078d4; }

    .actions { display: flex; justify-content: flex-end; align-items: center; gap: 15px; margin-top: 20px; }

    button { padding: 10px 24px; border: none; border-radius: 25px; cursor: pointer; font-weight: 600; font-size: 0.95rem; transition: background-color 0.2s, transform 0.1s; min-width: 120px; }
    button:active { transform: scale(0.98); }

    .cancel-btn { background-color: #e0e0e0; color: #333; }
    .cancel-btn:hover { background-color: #d0d0d0; }

    .submit-btn { background-color: #0078d4; color: white; }
    .submit-btn:hover:not(:disabled) { background-color: #005a9e; }
    .submit-btn:disabled { background-color: #a0cce8; cursor: not-allowed; }

    .message { padding: 10px 15px; border-radius: 8px; margin-top: 10px; }
    .error-message { background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
    ul { margin: 0; padding-left: 15px; }
    li { margin-bottom: 3px; }
  `]
})
export class UpdatePatientComponent implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cd = inject(ChangeDetectorRef);
  private toastService = inject(ToastService);

  patientId: number | null = null;
  
  firstName = '';
  lastName = '';
  uid = '';

  heightCm: number | null = null;
  weightKg: number | null = null;
  smokingStatus = 0;
  exerciseLevel = 0;
  dietType = 0;

  isLoading = true;
  isSaving = false;
  errorMessages: string[] = [];

  isSmokingOpen = false;
  isExerciseOpen = false;
  isDietOpen = false;

  smokingOptions = [
    { value: 0, label: 'Never' },
    { value: 1, label: 'Former' },
    { value: 2, label: 'Current' }
  ];

  exerciseOptions = [
    { value: 0, label: 'Sedentary' },
    { value: 1, label: 'Moderate' },
    { value: 2, label: 'Active' }
  ];

  dietOptions = [
    { value: 0, label: 'Regular' },
    { value: 1, label: 'Vegetarian' },
    { value: 2, label: 'Vegan' },
    { value: 3, label: 'Gluten Free' },
    { value: 4, label: 'Lactose Free' },
    { value: 5, label: 'Low Carb' },
    { value: 6, label: 'Low Sodium' },
    { value: 7, label: 'Diabetic' },
    { value: 8, label: 'Renal' },
    { value: 9, label: 'Soft' },
    { value: 10, label: 'Liquid' }
  ];

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.patientId = Number(id);
      this.fetchPatientData(this.patientId);
    } else {
      this.errorMessages = ['Invalid Patient ID'];
      this.isLoading = false;
    }
  }

  fetchPatientData(id: number) {
    this.http.get<any>(`/api/Patients/${id}`)
      .subscribe({
        next: (data) => {
          this.firstName = data.firstName;
          this.lastName = data.lastName;
          this.uid = data.uid;

          this.heightCm = data.heightCm;
          this.weightKg = data.weightKg;
          this.smokingStatus = data.smokingStatus;
          this.exerciseLevel = data.exerciseLevel;
          this.dietType = data.dietType;

          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.errorMessages = ['Failed to load data'];
          this.isLoading = false;
          this.cd.detectChanges();
        }
      });
  }

  getOptionLabel(options: any[], val: number) {
    return options.find(o => o.value === val)?.label || 'Select';
  }

  toggleSmoking(e: Event) { e.stopPropagation(); const open = !this.isSmokingOpen; this.closeAllDropdowns(); this.isSmokingOpen = open; }
  toggleExercise(e: Event) { e.stopPropagation(); const open = !this.isExerciseOpen; this.closeAllDropdowns(); this.isExerciseOpen = open; }
  toggleDiet(e: Event) { e.stopPropagation(); const open = !this.isDietOpen; this.closeAllDropdowns(); this.isDietOpen = open; }

  closeAllDropdowns() {
    this.isSmokingOpen = false;
    this.isExerciseOpen = false;
    this.isDietOpen = false;
  }

  selectOption(field: 'smokingStatus' | 'exerciseLevel' | 'dietType', val: number, event: Event) {
    event.stopPropagation();
    this[field] = val;
    this.closeAllDropdowns();
  }

  @HostListener('document:click')
  onClickOutside() {
    this.closeAllDropdowns();
  }

  clearMessages() {
    this.errorMessages = [];
  }

  onCancel() {
    this.router.navigate(['/patients', this.patientId]);
  }

  onSubmit() {
    if (!this.patientId) return;

    this.isSaving = true;
    this.clearMessages();

    const payload = {
      heightCm: this.heightCm,
      weightKg: this.weightKg,
      smokingStatus: this.smokingStatus,
      exerciseLevel: this.exerciseLevel,
      dietType: this.dietType
    };

    this.http.put(`/api/Patients/${this.patientId}`, payload)
      .subscribe({
        next: () => {
          this.isSaving = false;
          this.toastService.show('Action successful', 'success');
          this.router.navigate(['/patients', this.patientId]);
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.isSaving = false;
          this.toastService.show('Action failed', 'error');
          
          this.errorMessages = [];

          if (err.error) {
            if (err.error.errors) {
                const errorObj = err.error.errors;
                for (const key in errorObj) {
                    if (errorObj.hasOwnProperty(key)) {
                        this.errorMessages.push(...errorObj[key]);
                    }
                }
            } 
            else if (err.error.detail) {
                const rawMessages = err.error.detail.split(/,|\n/);
                this.errorMessages = rawMessages
                    .map((msg: string) => {
                        let cleanMsg = msg.replace(/^[^:]+:\s*/, '');
                        return cleanMsg.trim().replace(/\.$/, '');
                    })
                    .filter((msg: string) => msg.length > 0);
            }
          }

          if (this.errorMessages.length === 0) {
             this.errorMessages = ['Failed to update patient'];
          }
          
          this.cd.detectChanges();
        }
      });
  }
}