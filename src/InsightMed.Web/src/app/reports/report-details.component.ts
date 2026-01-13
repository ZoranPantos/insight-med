import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

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
  imports: [CommonModule],
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
        <button class="export-btn" (click)="onExportPdf()">Export PDF</button>
      </div>

      <div *ngIf="isLoading" class="loading">
        Loading report details...
      </div>

      <div *ngIf="errorMessage" class="error">
        {{ errorMessage }}
      </div>

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
    .export-btn:hover { background-color: #005a9e; }
    .export-btn:active { transform: scale(0.98); }

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

    .loading, .error { padding: 20px; text-align: center; color: #666; }
    .error { color: #d9534f; }
  `]
})
export class ReportDetailsComponent implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private cd = inject(ChangeDetectorRef);

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
          this.errorMessage = 'Failed to load report details.';
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
    console.log('Export PDF clicked for Report ID:', this.report?.id);
  }
}