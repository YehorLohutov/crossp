import { Injectable } from '@angular/core';
import {HttpClient, HttpEventType, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Project} from '../models/project';
import {Ad} from '../models/ad';
import { FileM } from '../models/file';
import {map} from 'rxjs/operators';
import {Token} from '../models/token';
import {CookieService} from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class CrosspService {
  protected readonly baseUrl = 'https://localhost:44389/';
  protected readonly cookieTokenName = 'token';
    //'https://crossp.azurewebsites.net/';
  protected token: Token = null;

  constructor(protected http: HttpClient, protected cookieService: CookieService) {
    if (this.cookieService.check(this.cookieTokenName)) {
      this.token = JSON.parse(this.cookieService.get(this.cookieTokenName));
    }
  }

  public login(username: string, password: string): Observable<boolean> {
    return this.http.get<Token>(`${this.baseUrl}users/token?username=${username}&password=${password}`)
      .pipe(
        map(result => {
          this.token = result;
          if (this.cookieService.check(this.cookieTokenName)) {
            this.cookieService.delete(this.cookieTokenName);
          }
          this.cookieService.set(this.cookieTokenName, JSON.stringify(result));
          return this.token !== null;
        })
      );
  }

  public logout(): void {
    this.token = null;
    if (this.cookieService.check(this.cookieTokenName)) {
      this.cookieService.delete(this.cookieTokenName);
    }
  }

  public userAuthenticated(): boolean {
    return this.token != null;
  }

  public getUserLogin(): string {
    return this.token.login;
  }

  public debug(): Observable<any> {
    const asda = { id: 99, name: 'adas'};
    return this.http.post<string>(this.baseUrl + 'api/projects', asda, {
      headers: { 'Content-Type': 'application/json'}
    });
  }

  public getFiles(userLogin): Observable<FileM[]> {
    return this.http.get<FileM[]>(this.baseUrl + 'Files/userlogin-' + userLogin).pipe(map(res => {
      try { return res; }
      catch (err) { console.log(err); return res; }
    }));
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

  public createAd(id): Observable<Ad> {
    return this.http.get<Ad>(this.baseUrl + 'Ads/Create/' + id);
  }

  public putAd(ad: Ad): Observable<any> {
    const headers = new HttpHeaders()
      .set('Content-Type', 'application/json');
    return this.http.put<Ad>(this.baseUrl + 'Ads/' + ad.id, ad, { headers });
  }

  public getFile(id): Observable<FileM> {
    return this.http.get<FileM>(this.baseUrl + 'Files/' + id);
  }

  public uploadFile(userLogin, files): Observable<any> {
    if (files.length === 0) {
      return;
    }
    const fileToUpload = files[0] as File;
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    return this.http.post(this.baseUrl + 'Files/uploadfile-' + userLogin, formData, { reportProgress: true, observe: 'events' });
  }

  public deleteFile(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + 'Files/' + id);
  }

  public getFileSrc(path): any {
    return this.baseUrl + path;
  }

  public isFileImage(file: FileM): boolean {
    return file.extension === '.png' || file.extension === '.jpg';
  }

  public isFileVideo(file: FileM): boolean {
    return file.extension === '.mp4';
  }
}
