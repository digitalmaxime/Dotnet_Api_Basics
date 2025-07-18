import { Component, inject, Signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { WeatherCardComponent } from './weather/weather-card/weather-card.component';
import { WeatherModel } from './weather/models/weather-model';
import { WeatherService } from './weather/services/weather.service';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, WeatherCardComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  weatherService = inject(WeatherService);

  weatherData: Signal<WeatherModel[]> = toSignal(this.weatherService.getWeather(), { initialValue: [] as WeatherModel[] });

}
