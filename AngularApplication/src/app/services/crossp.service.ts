import { Injectable } from '@angular/core';
import {HttpClient, HttpEventType, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Project} from '../models/project';
import {Ad} from '../models/ad';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CrosspService {
  protected baseUrl = 'https://crossp.azurewebsites.net/';

  constructor(protected http: HttpClient) {
  }

  public debug(): Observable<any> {
    const asda = { id: 99, name: 'adas'};
    return this.http.post<string>(this.baseUrl + 'api/projects', asda, {
      headers: { 'Content-Type': 'application/json'}
    });
  }

  public getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.baseUrl + 'Projects').pipe(map(res => {
      try { return res; }
      catch (err) { console.log(err); return res; }
    }));
  }

  public getProject(id): Observable<Project> {
    return this.http.get<Project>(this.baseUrl + 'Projects/' + id);
  }

  public createProject(): Observable<Project> {
    return this.http.get<Project>(this.baseUrl + 'Projects/Create');
  }

  public putProject(project: Project): Observable<any> {
    const headers = new HttpHeaders()
      .set('Content-Type', 'application/json');
    return this.http.put<Project>(this.baseUrl + 'Projects/' + project.id, project, { headers });
  }

  public deleteProject(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + 'Projects/' + id);
  }

  public getAds(projectId: number): Observable<Ad[]> {
    return this.http.get<Ad[]>(this.baseUrl + 'Ads/projectid-' + projectId);
  }

  public getAd(id): Observable<Ad> {
    return this.http.get<Ad>(this.baseUrl + 'Ads/' + id);
  }

  public putAd(ad: Ad): Observable<any> {
    const headers = new HttpHeaders()
      .set('Content-Type', 'application/json');
    return this.http.put<Ad>(this.baseUrl + 'Ads/' + ad.id, ad, { headers });
  }

  public uploadAdImage = (ad: Ad, files) => {
    if (files.length === 0) {
      return;
    }
    const fileToUpload = files[0] as File;
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    this.http.post(this.baseUrl + 'Ads/UploadAdImage/' + ad.id, formData, { reportProgress: true, observe: 'events' })
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
          // this.progress = Math.round(100 * event.loaded / event.total);
        }
        else if (event.type === HttpEventType.Response) {
          // this.message = 'Upload success.';
          // this.onUploadFinished.emit(event.body);
        }
      });
  }
}
