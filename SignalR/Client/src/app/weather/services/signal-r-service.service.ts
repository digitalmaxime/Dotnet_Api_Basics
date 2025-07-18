import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { WeatherModelShared } from '../models/weather-model-shared';

export enum ConnectionState {
  Disconnected = 'Disconnected',
  Connecting = 'Connecting',
  Connected = 'Connected'
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection;
  private connectionState = new BehaviorSubject<ConnectionState>(ConnectionState.Disconnected);
  private weatherUpdates = new Subject<WeatherModelShared[]>();

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7011/realtimehub', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Setup connection state change handlers
    this.hubConnection.onreconnecting(() => {
      console.log('SignalR reconnecting...');
      this.connectionState.next(ConnectionState.Connecting);
    });

    this.hubConnection.onreconnected(() => {
      console.log('SignalR reconnected');
      this.connectionState.next(ConnectionState.Connected);
    });

    this.hubConnection.onclose(() => {
      console.log('SignalR connection closed');
      this.connectionState.next(ConnectionState.Disconnected);
    });

    // Register handler for WeatherUpdated method
    // This matches your IWeatherForecast.WeatherUpdated method
    this.hubConnection.on('WeatherUpdated', (weatherData: any) => {
      console.log('Weather data received:', weatherData);
      this.weatherUpdates.next(weatherData);
    });
  }

  /**
   * Get the current connection state
   */
  public getConnectionState(): Observable<ConnectionState> {
    return this.connectionState.asObservable();
  }

  /**
   * Start the SignalR connection
   */
  public startConnection(): Observable<void> {
    if (this.connectionState.value !== ConnectionState.Disconnected) {
      return new Observable<void>(observer => {
        observer.next();
        observer.complete();
      });
    }

    this.connectionState.next(ConnectionState.Connecting);
    
    return new Observable<void>(observer => {
      this.hubConnection
        .start()
        .then(() => {
          console.log('Connection established with SignalR hub');
          this.connectionState.next(ConnectionState.Connected);
          observer.next();
          observer.complete();
        })
        .catch((error: any) => {
          console.error('Error connecting to SignalR hub:', error);
          this.connectionState.next(ConnectionState.Disconnected);
          observer.error(error);
        });
    });
  }

  /**
   * Stop the SignalR connection
   */
  public stopConnection(): Promise<void> {
    return this.hubConnection.stop()
      .then(() => {
        this.connectionState.next(ConnectionState.Disconnected);
        console.log('SignalR connection stopped');
      });
  }

  /**
   * Receive weather updates from the hub
   * This subscribes to the WeatherUpdated method from IWeatherForecast
   */
  public receiveWeatherUpdates(): Observable<WeatherModelShared[]> {
    return this.weatherUpdates.asObservable();
  }

  /**
   * Send a message to the hub
   * This calls the SendMessage method on your RealTimeHub
   */
  public sendMessage(message: string): Promise<void> {
    return this.hubConnection.invoke('SendMessage', message)
      .catch(err => {
        console.error('Error sending message:', err);
        throw err;
      });
  }
}
