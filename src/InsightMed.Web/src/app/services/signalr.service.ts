import { Injectable, signal, NgZone, inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection: HubConnection | undefined;
  private ngZone = inject(NgZone);
  
  public hasUnseenNotifications = signal<boolean>(false);

  constructor() { }

  public startConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/notifications')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR Connection Established with ID: ' + this.hubConnection?.connectionId);
        this.askServerForStatus();
      })
      .catch(err => console.error('Error while starting SignalR connection: ' + err));

    this.hubConnection.on('ReceiveUnseenStatus', (hasUnseen: boolean) => {
      this.ngZone.run(() => {
        this.hasUnseenNotifications.set(hasUnseen);
      });
      
    });
  }

  public askServerForStatus() {
    if (this.hubConnection?.state === 'Connected') {
      this.hubConnection.invoke('CheckUnseen')
        .catch(err => console.error(err));
    }
  }

  public stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}