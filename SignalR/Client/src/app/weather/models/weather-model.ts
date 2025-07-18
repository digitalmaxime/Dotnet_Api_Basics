export interface WeatherModel {
  temperature: number;
  condition: string;
  city: string;
  state: string;
  icon: string;
  date: Date;
  humidity: number;
  windSpeed: number;
}