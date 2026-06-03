import { Component, inject, HostListener, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-update-patient',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  templateUrl: './update-patient.component.html',
  styleUrl: './update-patient.component.css'
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
