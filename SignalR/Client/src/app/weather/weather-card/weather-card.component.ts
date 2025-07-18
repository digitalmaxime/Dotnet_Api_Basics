import { Component, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { WeatherModel } from '../models/weather-model';



@Component({
  selector: 'app-weather-card',
  imports: [DatePipe],
  templateUrl: './weather-card.component.html',
  styleUrl: './weather-card.component.css'
})

export class WeatherCardComponent {
  weather = input.required<WeatherModel>();
}
