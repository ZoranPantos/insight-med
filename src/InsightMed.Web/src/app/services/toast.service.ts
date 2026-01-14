import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type ToastType = 'success' | 'error';

export interface ToastState {
  message: string;
  type: ToastType;
  visible: boolean;
}

@Injectable({ 
  providedIn: 'root' 
})
export class ToastService {
  private state = new BehaviorSubject<ToastState>({
    message: '',
    type: 'success',
    visible: false
  });

  toastState$ = this.state.asObservable();
  private timeoutId: any;

  show(message: string, type: ToastType = 'success', duration: number = 3000) {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }

    this.state.next({
      message,
      type,
      visible: true
    });

    this.timeoutId = setTimeout(() => {
      this.hide();
    }, duration);
  }

  hide() {
    const current = this.state.value;
    this.state.next({ ...current, visible: false });
  }
}