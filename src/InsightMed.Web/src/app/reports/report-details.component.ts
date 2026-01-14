import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { LoadingSpinnerComponent } from '../shared/loading-spinner.component';
import { ErrorDisplayComponent } from '../shared/error-display.component';
import { ToastService } from '../services/toast.service';

interface ReferenceRange {
  MinThreshold?: number;
  MaxThreshold?: number;
  Positive?: boolean;
}

interface ReportItem {
  Id: number;
  Name: string;
  IsPositive?: boolean;
  Measurement?: number;
  Reference: ReferenceRange;
}

interface LabReportDetails {
  id: number;
  content: string; 
  created: string;
  patientFullName: string;
  patientUid: string;
}

@Component({
  selector: 'app-report-details',
  standalone: true,
  imports: [CommonModule, LoadingSpinnerComponent, ErrorDisplayComponent],
  template: `
    <div class="page-container">
      
      <div class="header">
        <div class="title-group">
            <h2>Report Details</h2>
            
            <div *ngIf="report" class="patient-info">
                <span class="patient-name">{{ report.patientFullName }}</span>
                <span class="uid-badge">{{ report.patientUid }}</span>
            </div>

            <span *ngIf="report" class="report-date">
              {{ report.created | date:'MMM d, y, h:mm a' }}
            </span>
        </div>
        <button class="export-btn" (click)="onExportPdf()" [disabled]="isLoading">
            {{ isLoading ? 'Exporting...' : 'Export PDF' }}
        </button>
      </div>

      <app-loading-spinner 
        *ngIf="isLoading" 
        minHeight="300px">
      </app-loading-spinner>

      <app-error-display
        *ngIf="errorMessage && !isLoading"
        [message]="errorMessage"
        minHeight="300px">
      </app-error-display>

      <div *ngIf="!isLoading && report && parsedContent.length > 0" class="table-container">
        <table>
          <thead>
            <tr>
              <th>Parameter</th>
              <th>Normal Range / Value</th>
              <th>Measured Value</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let item of parsedContent" [ngClass]="getRowClass(item)">
              <td>
                <span class="param-name">{{ item.Name }}</span>
              </td>

              <td>
                {{ getReferenceDisplay(item) }}
              </td>

              <td>
                <strong>{{ getMeasurementDisplay(item) }}</strong>
              </td>
            </tr>
          </tbody>
        </table>
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

    .report-date {
        color: #888;
        font-size: 0.9rem;
    }

    .export-btn {
      padding: 8px 20px;
      background-color: #0078d4; 
      color: white;
      border: none;
      border-radius: 20px;
      cursor: pointer;
      font-weight: 600; 
      font-size: 0.95rem;
      transition: background-color 0.2s, transform 0.1s;
    }
    .export-btn:hover:not(:disabled) { background-color: #005a9e; }
    .export-btn:active:not(:disabled) { transform: scale(0.98); }
    .export-btn:disabled { background-color: #a0cce8; cursor: not-allowed; }

    .table-container { border: 1px solid #e0e0e0; border-radius: 4px; overflow: hidden; }
    table { width: 100%; border-collapse: collapse; background: white; }
    th, td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #f0f0f0; }
    
    th {
      background-color: #fafafa; font-weight: 600; color: #555;
      font-size: 0.9em; text-transform: uppercase; letter-spacing: 0.5px;
    }
    
    .row-edge {
      background-color: #fff4ce; 
    }
    .row-abnormal {
      background-color: #f8d7da;
    }

    .param-name { font-weight: 500; color: #333; }
  `]
})
export class ReportDetailsComponent implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private cd = inject(ChangeDetectorRef);
  private toastService = inject(ToastService);

  report: LabReportDetails | null = null;
  parsedContent: ReportItem[] = [];
  
  isLoading = true;
  errorMessage = '';

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.fetchReport(id);
    } else {
      this.errorMessage = 'Invalid Report ID';
      this.isLoading = false;
    }
  }

  fetchReport(id: string) {
    this.http.get<LabReportDetails>(`http://localhost:5000/api/LabReports/${id}`)
      .subscribe({
        next: (data) => {
          this.report = data;
          this.parseContent(data.content);
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = 'Failed to load data';
          this.isLoading = false;
          this.cd.detectChanges();
        }
      });
  }

  parseContent(jsonString: string) {
    try {
      this.parsedContent = JSON.parse(jsonString);
    } catch (e) {
      console.error('Error parsing report content', e);
      this.errorMessage = 'Error processing report data.';
    }
  }

  getReferenceDisplay(item: ReportItem): string {
    const ref = item.Reference;
    if (ref.MinThreshold != null && ref.MaxThreshold != null) {
      return `${ref.MinThreshold} - ${ref.MaxThreshold}`;
    }
    if (ref.Positive != null) {
      return ref.Positive ? 'Positive' : 'Negative';
    }
    return 'N/A';
  }

  getMeasurementDisplay(item: ReportItem): string {
    if (item.Measurement != null) {
      return item.Measurement.toString();
    }
    if (item.IsPositive != null) {
      return item.IsPositive ? 'Positive' : 'Negative';
    }
    return '-';
  }

  getRowClass(item: ReportItem): string {
    const val = item.Measurement;
    const min = item.Reference.MinThreshold;
    const max = item.Reference.MaxThreshold;

    if (val != null && min != null && max != null) {
      if (val === min || val === max) {
        return 'row-edge';
      }

      if (val < min || val > max) {
        return 'row-abnormal';
      }
    }

    if (item.IsPositive != null && item.Reference.Positive != null) {
      if (item.IsPositive !== item.Reference.Positive) {
        return 'row-abnormal';
      }
    }

    return '';
  }

  onExportPdf() {
    if (!this.report) return;

    this.isLoading = true;
    this.cd.detectChanges();

    this.http.get(`http://localhost:5000/api/LabReports/${this.report.id}/export`, { 
      responseType: 'blob',
      observe: 'response' 
    }).subscribe({
      next: (response) => {
        const blob = response.body;
        if (blob) {
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          
          const contentDisposition = response.headers.get('content-disposition');
          let fileName = `report-${this.report?.id}.pdf`;
          if (contentDisposition) {
             const matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
             if (matches != null && matches[1]) { 
               fileName = matches[1].replace(/['"]/g, '');
             }
          }
          
          link.download = fileName;
          link.click();
          window.URL.revokeObjectURL(url);
          
          this.toastService.show('Action successful', 'success');
        }
        
        this.isLoading = false;
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error('Download failed', err);
        this.toastService.show('Action failed', 'error');
        
        this.errorMessage = "Could not download PDF";
        this.isLoading = false;
        this.cd.detectChanges();
      }
    });
  }
}