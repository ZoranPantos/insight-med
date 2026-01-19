import { Component, OnInit, inject, ChangeDetectorRef, ViewChild, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions, ScriptableContext } from 'chart.js'; 
import 'chartjs-adapter-date-fns';

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

interface LabParameterReference {
  minThreshold?: number;
  maxThreshold?: number;
  positive?: boolean;
}

interface HistoryItem {
  measurement?: number;
  isPositive?: boolean;
  created: string;
}

interface ParameterHistoryResponse {
  id: number;
  name: string;
  unit?: string;
  labParameterReference: LabParameterReference;
  history: HistoryItem[];
}

@Component({
  selector: 'app-parameter-analytics',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, LoadingSpinnerComponent, ErrorDisplayComponent, BaseChartDirective],
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

      <app-loading-spinner *ngIf="isLoadingInit" message="Loading data..." minHeight="300px"></app-loading-spinner>
      <app-error-display *ngIf="errorMessage && !isLoadingInit" [message]="errorMessage" minHeight="300px"></app-error-display>

      <div *ngIf="!isLoadingInit && !errorMessage" class="content">
        
        <div class="controls-card">
          <div class="form-group">
            <label>Select Parameter to Analyze</label>
            <div class="custom-select" (click)="toggleDropdown($event)" [class.open]="isDropdownOpen">
              <div class="selected-value">{{ getSelectedParameterName() }}</div>
              <span class="arrow">▼</span>

              <div *ngIf="isDropdownOpen" class="dropdown-list" (click)="$event.stopPropagation()">
                <div class="dropdown-search-wrapper">
                  <input type="text" class="dropdown-search-input" placeholder="Search..." [(ngModel)]="searchTerm" (input)="filterParameters()" autofocus />
                </div>
                <div class="options-container">
                  <div *ngFor="let param of filteredParameters" class="option-item" (click)="selectParameter(param)">
                    {{ param.name }}
                  </div>
                  <div *ngIf="filteredParameters.length === 0" class="empty-option">No parameters found</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div *ngIf="parameters.length === 0" class="empty-state">No evaluated parameters found.</div>

        <div *ngIf="selectedParameterId" class="analytics-card">
          
          <div *ngIf="isLoadingHistory" class="chart-loading">
            <app-loading-spinner message="Loading chart..." minHeight="200px"></app-loading-spinner>
          </div>

          <div *ngIf="!isLoadingHistory && chartData" class="chart-wrapper">
            <div class="chart-header">
                <h3>{{ historyData?.name }} History</h3>
                <span class="unit-badge" *ngIf="historyData?.unit">Unit: {{ historyData?.unit }}</span>
            </div>

            <canvas baseChart
              [data]="chartData"
              [options]="chartOptions"
              [type]="'line'">
            </canvas>

            <div class="reference-info" *ngIf="getReferenceText()">
               <span class="info-icon">ℹ️</span> Normal Range: <strong>{{ getReferenceText() }}</strong>
            </div>
          </div>

        </div>

      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 20px 0; font-family: sans-serif; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; border-bottom: 1px solid #eee; padding-bottom: 15px; }
    .title-group { display: flex; flex-direction: column; gap: 8px; }
    h2 { margin: 0; color: #333; }
    
    .patient-info { display: flex; align-items: center; gap: 10px; }
    .patient-name { font-size: 1.1rem; color: #666; }
    .uid-badge { background-color: #eef3fc; color: #3b5998; padding: 4px 8px; border-radius: 4px; font-weight: 500; font-size: 0.9em; }
    
    .back-link { color: #0078d4; text-decoration: none; font-weight: 500; font-size: 0.95rem; cursor: pointer; }
    .back-link:hover { text-decoration: underline; }

    .controls-card { background: white; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.02); }
    .form-group { max-width: 400px; }
    label { display: block; color: #666; font-size: 0.9em; font-weight: 500; margin-bottom: 8px; margin-left: 5px; }

    .custom-select { width: 100%; padding: 10px 20px; border: 1px solid #ccc; border-radius: 25px; background: white; cursor: pointer; position: relative; display: flex; justify-content: space-between; align-items: center; user-select: none; }
    .custom-select.open { border-color: #0078d4; box-shadow: 0 0 0 3px rgba(0,120,212,0.1); }
    .dropdown-list { position: absolute; top: 105%; left: 0; right: 0; background: white; border: 1px solid #ddd; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.15); z-index: 1000; overflow: hidden; display: flex; flex-direction: column; }
    .dropdown-search-wrapper { padding: 10px; border-bottom: 1px solid #eee; background: #fafafa; }
    .dropdown-search-input { width: 100%; padding: 8px 12px; border: 1px solid #ddd; border-radius: 6px; outline: none; box-sizing: border-box; }
    .options-container { max-height: 250px; overflow-y: auto; }
    .option-item { padding: 10px 20px; color: #333; transition: background 0.1s; cursor: pointer; }
    .option-item:hover { background-color: #f5f5f5; color: #0078d4; }
    .empty-option { padding: 15px; text-align: center; color: #999; font-style: italic; }

    .analytics-card { background: white; padding: 25px; border: 1px solid #e0e0e0; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.05); min-height: 350px; }
    .chart-wrapper { position: relative; height: 400px; width: 100%; }
    .chart-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; }
    .chart-header h3 { margin: 0; color: #444; }
    .unit-badge { background: #f0f0f0; padding: 4px 10px; border-radius: 12px; font-size: 0.85em; color: #666; font-weight: 600; }
    .reference-info { margin-top: 15px; padding: 10px; background-color: #f8f9fa; border-radius: 6px; color: #666; font-size: 0.9rem; text-align: center; }
    .info-icon { margin-right: 5px; }
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
  
  historyData: ParameterHistoryResponse | null = null;
  chartData: ChartConfiguration['data'] | undefined;
  chartOptions: ChartOptions = {};

  isDropdownOpen = false;
  searchTerm = '';
  isLoadingInit = true;
  isLoadingHistory = false;
  errorMessage = '';

  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  ngOnInit() {
    this.patientId = this.route.snapshot.paramMap.get('id');
    if (this.patientId) {
      this.loadInitData(this.patientId);
    } else {
      this.errorMessage = 'Invalid Patient ID';
      this.isLoadingInit = false;
    }
  }

  loadInitData(id: string) {
    this.isLoadingInit = true;
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
          this.fetchHistory(this.selectedParameterId);
        }

        this.isLoadingInit = false;
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load analytics data.';
        this.isLoadingInit = false;
        this.cd.detectChanges();
      }
    });
  }

  fetchHistory(parameterId: number) {
    if (!this.patientId) return;
    this.isLoadingHistory = true;
    this.chartData = undefined;

    this.http.get<ParameterHistoryResponse>(`http://localhost:5000/api/Patients/${this.patientId}/parameterHistory/${parameterId}`)
      .subscribe({
        next: (data) => {
          this.historyData = data;
          this.setupChart(data);
          this.isLoadingHistory = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.isLoadingHistory = false;
          this.cd.detectChanges();
        }
      });
  }

  setupChart(data: ParameterHistoryResponse) {
    const isNumeric = data.labParameterReference.minThreshold !== null;
    const sortedHistory = [...data.history].sort((a, b) => new Date(a.created).getTime() - new Date(b.created).getTime());
    const labels = sortedHistory.map(h => h.created);
    
    let datasetData: number[] = [];
    if (isNumeric) {
      datasetData = sortedHistory.map(h => h.measurement ?? 0);
    } else {
      datasetData = sortedHistory.map(h => h.isPositive === true ? 1 : 0);
    }

    const getPointColor = (ctx: ScriptableContext<'line'>) => {
      if (ctx.raw === undefined || ctx.raw === null) return 'gray';
      
      const val = ctx.raw as number;

      if (!isNumeric) {
        if (data.labParameterReference.positive === false) {
           return val === 1 ? '#d9534f' : '#5cb85c'; 
        }
        if (data.labParameterReference.positive === true) {
           return val === 1 ? '#5cb85c' : '#d9534f';
        }
        return 'gray';
      }

      const min = data.labParameterReference.minThreshold!;
      const max = data.labParameterReference.maxThreshold!;

      if (val < min || val > max) return '#d9534f';
      if (val === min || val === max) return '#f0ad4e';
      return '#5cb85c';
    };

    this.chartData = {
      labels: labels,
      datasets: [
        {
          data: datasetData,
          label: data.name,
          fill: false, 
          tension: isNumeric ? 0.4 : 0, 
          borderColor: '#9ca3af', 
          borderWidth: 2,
          stepped: !isNumeric, 
          
          pointBackgroundColor: getPointColor,
          pointBorderColor: getPointColor,
          pointRadius: 6,
          pointHoverRadius: 8
        }
      ]
    };

    const annotations: any = {};

    if (isNumeric) {
      const min = data.labParameterReference.minThreshold!;
      const max = data.labParameterReference.maxThreshold!;

      annotations.boxNormal = {
        type: 'box',
        yMin: min,
        yMax: max,
        backgroundColor: 'rgba(92, 184, 92, 0.1)', 
        borderWidth: 0
      };

      annotations.boxHigh = {
        type: 'box',
        yMin: max,
        yMax: 'Infinity', 
        backgroundColor: 'rgba(217, 83, 79, 0.1)', 
        borderWidth: 0
      };

      annotations.boxLow = {
        type: 'box',
        yMin: '-Infinity', 
        yMax: min,
        backgroundColor: 'rgba(217, 83, 79, 0.1)', 
        borderWidth: 0
      };
    }

    this.chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        x: {
          type: 'time',
          time: {
            unit: 'day',
            tooltipFormat: 'MMM d, yyyy HH:mm',
            displayFormats: { day: 'MMM d' }
          },
          title: { display: true, text: 'Date' }
        },
        y: {
          title: { display: true, text: isNumeric ? `Value (${data.unit || ''})` : 'Result' },
          ticks: isNumeric ? {} : {
            callback: (val) => val === 0 ? 'Negative' : (val === 1 ? 'Positive' : '')
          },
          grace: '10%' 
        }
      },
      plugins: {
        legend: { display: false },
        tooltip: {
          callbacks: {
            label: (context) => {
              if (!isNumeric) {
                return context.raw === 1 ? 'Positive' : 'Negative';
              }
              return `${context.raw} ${data.unit || ''}`;
            }
          }
        },
        annotation: {
          annotations: annotations
        }
      }
    };
  }

  getReferenceText(): string {
    if (!this.historyData) return '';
    const ref = this.historyData.labParameterReference;
    if (ref.minThreshold !== null && ref.maxThreshold !== null) {
      return `${ref.minThreshold} - ${ref.maxThreshold} ${this.historyData.unit || ''}`;
    }
    if (ref.positive !== null) {
      return ref.positive ? 'Positive' : 'Negative';
    }
    return '';
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
      this.filteredParameters = this.parameters.filter(p => p.name.toLowerCase().includes(term));
    }
  }

  selectParameter(param: EvaluatedParameter) {
    this.selectedParameterId = param.id;
    this.isDropdownOpen = false;
    this.fetchHistory(param.id);
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
}