import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { WeatherModel } from '../models/weather-model';
import { WeatherModelShared } from '../models/weather-model-shared';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7011/api/weatherforecast';

  getWeather(): Observable<WeatherModel[]> {
    return this.http
      .get<WeatherModelShared[]>(this.apiUrl)
      .pipe(map(weather => weather.map(shared => this.mapToWeatherModel(shared))),
        catchError(error => {
          console.error('Error fetching weather data:', error);
          return of([]);
        }) 
      );
  }

  public mapToWeatherModel(shared: WeatherModelShared): WeatherModel {
    return {
      city: shared.city || "Montreal",
      state: shared.state || "Quebec",
      temperature: shared.temperatureC,
      condition: shared.condition,
      humidity: shared.humidity || 0,
      windSpeed: shared.windSpeed || 0,
      icon: this.getWeatherIcon(shared.condition),
      date: new Date(shared.date)
    };
  }

  private getWeatherIcon(condition: string): string {
    switch (condition?.toLowerCase()) {
      case 'sunny': return '☀️';
      case 'cloudy': return '☁️';
      case 'rainy': return '🌧️';
      default: return '❓';
    }
  }

}