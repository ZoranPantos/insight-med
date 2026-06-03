import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { ErrorDisplayComponent } from '../../shared/error-display/error-display.component';
import { ToastService } from '../../services/toast.service';

interface ReferenceRange {
  MinThreshold?: number;
  MaxThreshold?: number;
  Positive?: boolean;
  Unit?: string;
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
  templateUrl: './report-details.component.html',
  styleUrl: './report-details.component.css'
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
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.fetchReport(id);
      } else {
        this.errorMessage = 'Invalid Report ID';
        this.isLoading = false;
      }
    });
  }

  fetchReport(id: string) {
    this.isLoading = true;
    this.report = null;
    this.errorMessage = '';

    this.http.get<LabReportDetails>(`/api/LabReports/${id}`)
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

    this.http.get(`/api/LabReports/${this.report.id}/export`, { 
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
