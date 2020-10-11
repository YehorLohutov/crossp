import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class CrosspService {


  constructor(protected http: HttpClient, @Inject('BASE_URL') protected baseUrl: string) {
  }

  public getWeatherForecasts(): Observable<WeatherForecast[]> {
    return this.http.get<WeatherForecast[]>(this.baseUrl + 'weatherforecast');
  }

  public debug(): Observable<any> {
    let asda = { id: 99, name: 'adas'};
    return this.http.post<string>(this.baseUrl + 'api/projects', asda, {
      headers: { 'Content-Type': 'application/json'}
    });
  }

  public getStr() {
    return 'adasda';
  }

  public getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.baseUrl + 'api/Projects');
  }

  public getProject(id): Observable<Project> {
    return this.http.get<Project>(this.baseUrl + 'api/Projects/' + id);
  }

  public createProject(): Observable<Project> {
    return this.http.get<Project>(this.baseUrl + 'api/Projects/Create');
  }

  public deleteProject(id: number): Observable<Project> {
    return this.http.delete(this.baseUrl + 'api/Projects/' + id);
  }
}
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
