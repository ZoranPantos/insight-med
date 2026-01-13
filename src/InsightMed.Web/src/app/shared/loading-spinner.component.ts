import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="spinner-container" [ngStyle]="{'min-height': minHeight}">
      <div class="spinner"></div>
      <div *ngIf="message" class="message">{{ message }}</div>
    </div>
  `,
  styles: [`
    .spinner-container {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      padding: 20px;
      width: 100%;
    }

    .spinner {
      width: 40px;
      height: 40px;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #0078d4;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    .message {
      margin-top: 15px;
      color: #666;
      font-size: 0.9rem;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
  `]
})
export class LoadingSpinnerComponent {
  @Input() message: string = 'Loading...';
  @Input() minHeight: string = 'auto';
}