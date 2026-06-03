import { Component, OnInit, inject, ChangeDetectorRef, ViewChild, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions, ScriptableContext } from 'chart.js'; 

import { LoadingSpinnerComponent } from '../shared/loading-spinner/loading-spinner.component';
import { ErrorDisplayComponent } from '../shared/error-display/error-display.component';

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
  templateUrl: './parameter-analytics.component.html',
  styleUrl: './parameter-analytics.component.css'
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
      patient: this.http.get<PatientLite>(`/api/Patients/${id}`),
      analytics: this.http.get<AnalyticsResponse>(`/api/Patients/${id}/evaluatedParameters`)
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

    this.http.get<ParameterHistoryResponse>(`/api/Patients/${this.patientId}/parameterHistory/${parameterId}`)
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
    
    const labels = sortedHistory.map(h => {
        const d = new Date(h.created);
        const dateStr = d.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
        const timeStr = d.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
        return [dateStr, timeStr]; 
    });
    
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
      labels: labels as any,
      datasets: [
        {
          data: datasetData,
          label: data.name,
          fill: false, 
          tension: isNumeric ? 0.3 : 0,
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
          type: 'category', 
          title: { display: true, text: 'Date' },
          ticks: {
            maxRotation: 0,
            autoSkip: true
          }
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
            title: (tooltipItems) => {
                const index = tooltipItems[0].dataIndex;
                const labelArray = this.chartData?.labels?.[index];
                
                if (Array.isArray(labelArray)) {
                    return labelArray.join(' at ');
                }
                return tooltipItems[0].label;
            },
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
