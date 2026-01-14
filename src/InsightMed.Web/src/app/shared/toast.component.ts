import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from '../services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="toastService.toastState$ | async as toast" 
         class="toast-container" 
         [class.visible]="toast.visible"
         [class.success]="toast.type === 'success'"
         [class.error]="toast.type === 'error'">
      
      <div class="toast-content">
        <span *ngIf="toast.type === 'success'" class="icon">✅</span>
        
        <span *ngIf="toast.type === 'error'" class="icon">⚠️</span>

        <span class="message">{{ toast.message }}</span>
      </div>

      <button class="close-btn" (click)="close()">×</button>
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      bottom: 20px;
      left: 50%;
      transform: translateX(-50%) translateY(100px);
      display: flex;
      align-items: center;
      justify-content: space-between;
      min-width: 300px;
      padding: 12px 20px;
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
      z-index: 9999;
      opacity: 0;
      transition: all 0.3s ease-in-out;
      font-family: "Segoe UI", sans-serif;
    }

    .toast-container.visible {
      transform: translateX(-50%) translateY(0);
      opacity: 1;
    }

    .toast-container.success {
      background-color: #d4edda;
      border-left: 5px solid #28a745;
      color: #155724;
    }

    .toast-container.error {
      background-color: #f8d7da;
      border-left: 5px solid #dc3545;
      color: #721c24;
    }

    .toast-content {
      display: flex;
      align-items: center;
      gap: 10px;
    }

    .message {
      font-weight: 500;
      font-size: 0.95rem;
    }

    .close-btn {
      background: none;
      border: none;
      font-size: 1.2rem;
      cursor: pointer;
      color: inherit;
      opacity: 0.6;
      margin-left: 15px;
      line-height: 1;
      padding: 0 5px;
    }
    
    .close-btn:hover {
      opacity: 1;
    }
  `]
})
export class ToastComponent {
  toastService = inject(ToastService);

  close() {
    this.toastService.hide();
  }
}