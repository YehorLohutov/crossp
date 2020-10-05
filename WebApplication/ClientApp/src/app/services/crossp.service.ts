import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CrosspService {


  constructor(protected http: HttpClient, @Inject('BASE_URL') protected baseUrl: string) {
  }

  public getWeatherForecasts(): Observable<WeatherForecast[]> {
    return this.http.get<WeatherForecast[]>(this.baseUrl + 'weatherforecast');
  }

  public getStr() {
    return 'adasda';
  }

  public getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.baseUrl + 'api/Projects');
  }

}
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
