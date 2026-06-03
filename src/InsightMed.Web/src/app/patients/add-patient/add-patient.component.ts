import { Component, inject, HostListener, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-add-patient',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  templateUrl: './add-patient.component.html',
  styleUrl: './add-patient.component.css'
})
export class AddPatientComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);
  private cd = inject(ChangeDetectorRef);
  private toastService = inject(ToastService);

  firstName = '';
  lastName = '';
  uid = '';
  email = '';
  phone = '';
  dateOfBirth = '';
  
  gender = 0; 
  bloodGroup = 0; 
  
  heightCm: number | null = null;
  weightKg: number | null = null;
  smokingStatus = 0;
  exerciseLevel = 0;
  dietType = 0;

  isLoading = false;
  errorMessages: string[] = [];

  isGenderOpen = false;
  isBloodGroupOpen = false;
  isDateOpen = false;
  isSmokingOpen = false;
  isExerciseOpen = false;
  isDietOpen = false;

  viewYear = new Date().getFullYear();
  viewMonth = new Date().getMonth();
  monthDays: number[] = [];
  emptyDays: any[] = [];
  months = ["January","February","March","April","May","June","July","August","September","October","November","December"];

  genderOptions = [
    { value: 0, label: 'Male' },
    { value: 1, label: 'Female' }
  ];

  bloodGroupOptions = [
    { value: 0, label: 'A Positive' },
    { value: 1, label: 'A Negative' },
    { value: 2, label: 'B Positive' },
    { value: 3, label: 'B Negative' },
    { value: 4, label: 'AB Positive' },
    { value: 5, label: 'AB Negative' },
    { value: 6, label: 'O Positive' },
    { value: 7, label: 'O Negative' }
  ];

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
    this.generateCalendar();
  }

  toggleDate(event: Event) {
    event.stopPropagation();
    this.closeAllDropdowns();
    this.isDateOpen = !this.isDateOpen;
    
    if (this.isDateOpen) {
      const d = this.dateOfBirth ? new Date(this.dateOfBirth) : new Date();
      this.viewYear = d.getFullYear();
      this.viewMonth = d.getMonth();
      this.generateCalendar();
    }
  }

  changeMonth(delta: number) {
    this.viewMonth += delta;
    if (this.viewMonth > 11) {
      this.viewMonth = 0;
      this.viewYear++;
    } else if (this.viewMonth < 0) {
      this.viewMonth = 11;
      this.viewYear--;
    }
    this.generateCalendar();
  }

  generateCalendar() {
    const firstDay = new Date(this.viewYear, this.viewMonth, 1).getDay();
    const daysInMonth = new Date(this.viewYear, this.viewMonth + 1, 0).getDate();
    this.emptyDays = Array(firstDay).fill(0);
    this.monthDays = Array.from({ length: daysInMonth }, (_, i) => i + 1);
  }

  selectDate(day: number) {
    const m = (this.viewMonth + 1).toString().padStart(2, '0');
    const d = day.toString().padStart(2, '0');
    this.dateOfBirth = `${this.viewYear}-${m}-${d}`;
    this.isDateOpen = false;
    this.clearMessages();
  }

  isSelected(day: number): boolean {
    if (!this.dateOfBirth) return false;
    const [y, m, d] = this.dateOfBirth.split('-').map(Number);
    return y === this.viewYear && m === (this.viewMonth + 1) && d === day;
  }

  getMonthName(idx: number) { return this.months[idx]; }

  getOptionLabel(options: any[], val: number) {
    return options.find(o => o.value === val)?.label || 'Select';
  }

  toggleGender(e: Event) { e.stopPropagation(); const open = !this.isGenderOpen; this.closeAllDropdowns(); this.isGenderOpen = open; }
  toggleBlood(e: Event) { e.stopPropagation(); const open = !this.isBloodGroupOpen; this.closeAllDropdowns(); this.isBloodGroupOpen = open; }
  toggleSmoking(e: Event) { e.stopPropagation(); const open = !this.isSmokingOpen; this.closeAllDropdowns(); this.isSmokingOpen = open; }
  toggleExercise(e: Event) { e.stopPropagation(); const open = !this.isExerciseOpen; this.closeAllDropdowns(); this.isExerciseOpen = open; }
  toggleDiet(e: Event) { e.stopPropagation(); const open = !this.isDietOpen; this.closeAllDropdowns(); this.isDietOpen = open; }

  closeAllDropdowns() {
    this.isGenderOpen = false;
    this.isBloodGroupOpen = false;
    this.isDateOpen = false;
    this.isSmokingOpen = false;
    this.isExerciseOpen = false;
    this.isDietOpen = false;
  }

  selectOption(field: 'gender' | 'bloodGroup' | 'smokingStatus' | 'exerciseLevel' | 'dietType', val: number, event: Event) {
    event.stopPropagation();
    this[field] = val;
    this.closeAllDropdowns();
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    this.closeAllDropdowns();
  }

  clearMessages() {
    this.errorMessages = [];
  }

  onCancel() {
    this.router.navigate(['/patients']);
  }

  onSubmit() {
    if (!this.firstName || !this.lastName || !this.uid || !this.email || !this.dateOfBirth) {
      this.errorMessages = ['Please fill in all required fields'];
      return;
    }

    const selectedDate = new Date(this.dateOfBirth);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (selectedDate > today) {
      this.errorMessages = ['Date of birth cannot be in the future'];
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const payload = {
      firstName: this.firstName,
      lastName: this.lastName,
      uid: this.uid,
      email: this.email,
      phone: this.phone,
      dateOfBirth: this.dateOfBirth,
      gender: this.gender,
      bloodGroup: this.bloodGroup,
      heightCm: this.heightCm,
      weightKg: this.weightKg,
      smokingStatus: this.smokingStatus,
      exerciseLevel: this.exerciseLevel,
      dietType: this.dietType
    };

    this.http.post('/api/Patients', payload)
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.toastService.show('Action successful', 'success');
          this.router.navigate(['/patients']);
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.isLoading = false;
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
             this.errorMessages = ['Failed to add patient'];
          }
          
          this.cd.detectChanges();
        }
      });
  }
}
