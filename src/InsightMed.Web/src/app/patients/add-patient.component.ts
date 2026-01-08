import { Component, inject, HostListener, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-patient',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="header">
        <h2>Add New Patient</h2>
      </div>

      <div class="form-container">
        
        <div class="form-row">
          <div class="form-group">
            <label>First Name</label>
            <input type="text" [(ngModel)]="firstName" (input)="clearMessages()" class="pill-input" />
          </div>
          <div class="form-group">
            <label>Last Name</label>
            <input type="text" [(ngModel)]="lastName" (input)="clearMessages()" class="pill-input" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>UID (Unique ID)</label>
            <input type="text" [(ngModel)]="uid" (input)="clearMessages()" class="pill-input" />
          </div>
          <div class="form-group">
            <label>Phone</label>
            <input type="text" [(ngModel)]="phone" (input)="clearMessages()" class="pill-input" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Email</label>
            <input type="email" [(ngModel)]="email" (input)="clearMessages()" class="pill-input" />
          </div>
          
          <div class="form-group">
            <label>Date of Birth</label>
            <div class="custom-select" (click)="toggleDate($event)" [class.open]="isDateOpen">
              <div class="selected-value" [class.placeholder]="!dateOfBirth">
                {{ dateOfBirth ? (dateOfBirth | date:'mediumDate') : 'Select Date' }}
              </div>
              <span class="icon">📅</span>

              <div *ngIf="isDateOpen" class="dropdown-list calendar-dropdown" (click)="$event.stopPropagation()">
                
                <div class="calendar-header">
                  <button (click)="changeMonth(-1)">‹</button>
                  <span>{{ getMonthName(viewMonth) }} {{ viewYear }}</span>
                  <button (click)="changeMonth(1)">›</button>
                </div>

                <div class="calendar-grid days-header">
                  <span>Su</span><span>Mo</span><span>Tu</span><span>We</span><span>Th</span><span>Fr</span><span>Sa</span>
                </div>

                <div class="calendar-grid">
                  <span *ngFor="let empty of emptyDays" class="day empty"></span>
                  <span *ngFor="let day of monthDays" 
                        class="day" 
                        [class.selected]="isSelected(day)"
                        (click)="selectDate(day)">
                    {{ day }}
                  </span>
                </div>
              </div>
            </div>
          </div>

        </div>

        <div class="form-row">
          
          <div class="form-group">
            <label>Gender</label>
            <div class="custom-select" (click)="toggleGender($event)" [class.open]="isGenderOpen">
              <div class="selected-value">{{ getGenderLabel(gender) }}</div>
              <span class="arrow">▼</span>
              
              <div *ngIf="isGenderOpen" class="dropdown-list">
                <div *ngFor="let opt of genderOptions" class="option-item" (click)="selectGender(opt.value, $event)">
                  {{ opt.label }}
                </div>
              </div>
            </div>
          </div>

          <div class="form-group">
            <label>Blood Group</label>
            <div class="custom-select" (click)="toggleBlood($event)" [class.open]="isBloodGroupOpen">
              <div class="selected-value">{{ getBloodGroupLabel(bloodGroup) }}</div>
              <span class="arrow">▼</span>

              <div *ngIf="isBloodGroupOpen" class="dropdown-list">
                <div *ngFor="let opt of bloodGroupOptions" class="option-item" (click)="selectBlood(opt.value, $event)">
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
          
          <button class="submit-btn" (click)="onSubmit()" [disabled]="isLoading">
            Save
          </button>
        </div>

      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; max-width: 800px; margin: 0 auto; }
    .header { margin-bottom: 25px; border-bottom: 1px solid #eee; padding-bottom: 15px; }
    h2 { margin: 0; color: #333; }

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
    .arrow, .icon { font-size: 0.8em; color: #666; }

    .dropdown-list {
      position: absolute; top: 105%; left: 0; right: 0;
      background: white; border: 1px solid #ddd; border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15); z-index: 1000;
      overflow: hidden; max-height: 320px; overflow-y: auto;
    }

    .option-item { padding: 10px 20px; color: #333; transition: background 0.1s; }
    .option-item:hover { background-color: #f5f5f5; color: #0078d4; }

    .calendar-dropdown { padding: 15px; cursor: default; }
    
    .calendar-header {
      display: flex; justify-content: space-between; align-items: center;
      margin-bottom: 10px; font-weight: 600; color: #333;
    }
    .calendar-header button {
      background: none; border: none; font-size: 1.2rem; 
      color: #666; cursor: pointer; min-width: auto; padding: 0 5px;
    }
    .calendar-header button:hover { color: #0078d4; background: none; }

    .calendar-grid {
      display: grid; grid-template-columns: repeat(7, 1fr);
      text-align: center; row-gap: 5px;
    }
    
    .days-header span {
      font-size: 0.8em; color: #888; font-weight: 600; margin-bottom: 5px;
    }

    .day {
      width: 30px; height: 30px; line-height: 30px;
      margin: 0 auto; border-radius: 50%; font-size: 0.9em;
      cursor: pointer; transition: background 0.2s;
    }
    .day:hover:not(.empty) { background-color: #f0f0f0; }
    .day.selected { background-color: #0078d4; color: white; }
    .day.empty { cursor: default; }

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

    .message { padding: 10px 15px; border-radius: 8px; margin-top: 10px; }
    .error-message { background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
    ul { margin: 0; padding-left: 15px; }
    li { margin-bottom: 3px; }
    li:last-child { margin-bottom: 0; }
  `]
})
export class AddPatientComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);
  private cd = inject(ChangeDetectorRef);

  firstName = '';
  lastName = '';
  uid = '';
  email = '';
  phone = '';
  dateOfBirth = '';
  
  gender: number = 0; 
  bloodGroup: number = 0; 

  isLoading = false;
  errorMessages: string[] = [];

  isGenderOpen = false;
  isBloodGroupOpen = false;
  isDateOpen = false;

  viewYear: number = new Date().getFullYear();
  viewMonth: number = new Date().getMonth();
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

  ngOnInit() {
    this.generateCalendar();
  }

  toggleDate(event: Event) {
    event.stopPropagation();
    this.isDateOpen = !this.isDateOpen;
    this.isGenderOpen = false;
    this.isBloodGroupOpen = false;
    
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

  getMonthName(idx: number) {
    return this.months[idx];
  }

  getGenderLabel(val: number) {
    return this.genderOptions.find(o => o.value === val)?.label || 'Select';
  }
  getBloodGroupLabel(val: number) {
    return this.bloodGroupOptions.find(o => o.value === val)?.label || 'Select';
  }

  toggleGender(event: Event) {
    event.stopPropagation();
    this.isGenderOpen = !this.isGenderOpen;
    this.isBloodGroupOpen = false;
    this.isDateOpen = false;
  }

  toggleBlood(event: Event) {
    event.stopPropagation();
    this.isBloodGroupOpen = !this.isBloodGroupOpen;
    this.isGenderOpen = false;
    this.isDateOpen = false;
  }

  selectGender(val: number, event: Event) {
    event.stopPropagation();
    this.gender = val;
    this.isGenderOpen = false;
  }

  selectBlood(val: number, event: Event) {
    event.stopPropagation();
    this.bloodGroup = val;
    this.isBloodGroupOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    this.isGenderOpen = false;
    this.isBloodGroupOpen = false;
    this.isDateOpen = false;
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
      bloodGroup: this.bloodGroup
    };

    this.http.post('http://localhost:5000/api/Patients', payload)
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/patients']);
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.isLoading = false;
          if (err.error && err.error.detail) {
            const rawMessages = err.error.detail.split(',');
            this.errorMessages = rawMessages.map((msg: string) => msg.trim().replace(/\.$/, ''));
          } else {
            this.errorMessages = ['Failed to add patient'];
          }
          this.cd.detectChanges();
        }
      });
  }
}