import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { WeatherCardComponent } from './weather/weather-card/weather-card.component';
import { WeatherModel } from './weather/models/weather-model';
import { WeatherService } from './weather/services/weather.service';
import { SignalRService, ConnectionState } from './weather/services/signal-r-service.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, WeatherCardComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit, OnDestroy {
  private weatherService = inject(WeatherService);
  private signalRService = inject(SignalRService);

  weatherData: WeatherModel[] = [];
  connectionState: ConnectionState = ConnectionState.Disconnected;

  private subscriptions: Subscription[] = [];

  ngOnInit(): void {
    // Get initial weather data
    this.subscriptions.push(
      this.weatherService.getWeather().subscribe(data => {
        this.weatherData = data;
      })
    );

    // Track connection state
    this.subscriptions.push(
      this.signalRService.getConnectionState().subscribe(state => {
        this.connectionState = state;
      })
    );

    // Connect to SignalR hub
    this.signalRService.startConnection().subscribe({
      next: () => {
        console.log('Successfully connected to SignalR hub');

        // Listen for real-time weather updates
        this.subscriptions.push(
          this.signalRService.receiveWeatherUpdates().subscribe(updates => {
            console.log('Received weather updates:', updates);
            if (updates && updates.length > 0) {
              this.weatherData = updates.map(update => this.weatherService.mapToWeatherModel(update));
            }
          })
        );
      },
      error: (err) => console.error('Failed to connect to SignalR hub:', err)
    });
  }

  // Method to request new data
  refreshWeather(): void {
    this.signalRService.sendMessage('Please send updated weather data');
  }

  // Method to reconnect if disconnected
  reconnect(): void {
    if (this.connectionState === ConnectionState.Disconnected) {
      this.signalRService.startConnection().subscribe();
    }
  }

  ngOnDestroy(): void {
    // Clean up subscriptions
    this.subscriptions.forEach(subscription => subscription.unsubscribe());

    // Stop SignalR connection
    this.signalRService.stopConnection();
  }
}
