export interface WeatherModelShared {
  date: string;
  temperatureC: number;
  temperatureF: number;
  condition: string;
  city?: string;
  state?: string;
  humidity?: number;
  windSpeed?: number;
}