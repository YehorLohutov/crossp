import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpEventType } from '@angular/common/http';
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
    return this.http.get<Project[]>(this.baseUrl + 'api/Projects').pipe(map(res => {
      try { return res; }
      catch (err) { console.log(err); return res; }
    }));
  }

  public getProject(id): Observable<Project> {
    return this.http.get<Project>(this.baseUrl + 'api/Projects/' + id);
  }

  public createProject(): Observable<Project> {
    return this.http.get<Project>(this.baseUrl + 'api/Projects/Create');
  }

  public putProject(project: Project): Observable<any> {
    const headers = new HttpHeaders()
      .set("Content-Type", "application/json");
    return this.http.put<Project>(this.baseUrl + 'api/Projects/' + project.id, project, { headers });
  }

  public deleteProject(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + 'api/Projects/' + id);
  }

  public getAds(projectId: number): Observable<Ad[]> {
    return this.http.get<Ad[]>(this.baseUrl + 'api/Ads/projectid-' + projectId);
  }

  public putAd(ad: Ad): Observable<any> {
    const headers = new HttpHeaders()
      .set("Content-Type", "application/json");
    return this.http.put<Ad>(this.baseUrl + 'api/Ads/' + ad.id, ad, { headers });
  }

  public uploadAdImage = (ad: Ad, files) => {
    if (files.length === 0) {
      return;
    }
    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    this.http.post(this.baseUrl + 'api/Ads/UploadAdImage/' + ad.id, formData, { reportProgress: true, observe: 'events' })
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
          //this.progress = Math.round(100 * event.loaded / event.total);
        }
        else if (event.type === HttpEventType.Response) {
          //this.message = 'Upload success.';
          //this.onUploadFinished.emit(event.body);
        }
      });
  }

}
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
