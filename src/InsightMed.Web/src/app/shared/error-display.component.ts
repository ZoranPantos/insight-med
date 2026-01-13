import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-display',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="error-container" [ngStyle]="{'min-height': minHeight}">
      <div class="icon-wrapper">
        <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <circle cx="12" cy="12" r="10"></circle>
          <path d="M16 16s-1.5-2-4-2-4 2-4 2"></path>
          <line x1="9" y1="9" x2="9.01" y2="9"></line>
          <line x1="15" y1="9" x2="15.01" y2="9"></line>
        </svg>
      </div>
      
      <p class="error-message">{{ message }}</p>
    </div>
  `,
  styles: [`
    .error-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      text-align: center;
      padding: 40px 20px;
      color: #d9534f;
    }

    .icon-wrapper {
      margin-bottom: 5px;
      opacity: 0.8;
    }

    .error-message {
      margin: 0;
      font-size: 1rem;
      font-weight: 500;
    }
  `]
})
export class ErrorDisplayComponent {
  @Input() message: string = 'An unexpected error occurred';
  @Input() minHeight: string = 'auto';
}