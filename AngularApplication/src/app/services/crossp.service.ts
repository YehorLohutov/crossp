import { Injectable } from '@angular/core';
import {HttpClient, HttpEventType, HttpHeaders} from '@angular/common/http';
import {from, Observable} from 'rxjs';
import {Project} from '../models/project';
import {Ad} from '../models/ad';
import { FileM } from '../models/file';
import {map} from 'rxjs/operators';
import {Token} from '../models/token';
import {CookieService} from 'ngx-cookie-service';
import { AdClicksStats } from '../models/adclicksstats'
import { AdShowStats } from '../models/adshowstats'

@Injectable({
  providedIn: 'root'
})
export class CrosspService {
  protected readonly baseUrl = 'https://crossp.azurewebsites.net/';
  protected readonly cookieTokenName = 'token';
  protected readonly apiVersion = "1.0";
  protected headers: HttpHeaders;

  protected token: Token = null;

  constructor(protected http: HttpClient, protected cookieService: CookieService) {
    if (this.cookieService.check(this.cookieTokenName)) {
      this.token = JSON.parse(this.cookieService.get(this.cookieTokenName));
      this.headers = new HttpHeaders()
        .set('Authorization', 'Bearer ' + this.token.accessToken)
        .set('Content-Type', 'application/json');
    }
  }

  public login(username: string, password: string): Observable<boolean> {
    return this.http.get<Token>(`${this.baseUrl}users/token?api-version=${this.apiVersion}&username=${username}&password=${password}`)
      .pipe(
        map(result => {
          this.token = result;
          console.log(this.token);
          this.headers = new HttpHeaders()
            .set('Authorization', 'Bearer ' + this.token.accessToken)
            .set('Content-Type', 'application/json');
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

  public getUserId(): string {
    return this.token.id;
  }

  // public debug(): Observable<any> {
  //   const asda = { id: 99, name: 'adas'};
  //   return this.http.post<string>(this.baseUrl + 'api/projects', asda, {
  //     headers: { 'Content-Type': 'application/json'}
  //   });
  // }

  public getFiles(userId): Observable<FileM[]> {
    console.log(this.token);
    return this.http.get<FileM[]>(`${this.baseUrl}Files/userfiles?api-version=${this.apiVersion}&userId=${userId}`, { headers: this.headers }).pipe(map(res => {
      try { return res; }
      catch (err) { console.log(err); return res; }
    }));
  }

  public getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.baseUrl}Projects?api-version=${this.apiVersion}`, { headers: this.headers }).pipe(map(res => {
      try { return res; }
      catch (err) { console.log(err); return res; }
    }));
  }

  public getProject(id): Observable<Project> {
    return this.http.get<Project>(`${this.baseUrl}Projects/${id}?api-version=${this.apiVersion}`, { headers: this.headers });
  }

  public createProject(userId): Observable<Project> {
    return this.http.get<Project>(`${this.baseUrl}Projects/create?api-version=${this.apiVersion}&userid=${userId}`, { headers: this.headers });
  }

  public putProject(project: Project): Observable<any> {
    // const headers = new HttpHeaders()
    //  .set('Content-Type', 'application/json');
    return this.http.put<Project>(`${this.baseUrl}Projects/${project.id}?api-version=${this.apiVersion}`, project, { headers: this.headers });
  }

  public deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}Projects/${id}?api-version=${this.apiVersion}`, { headers: this.headers });
  }

  public getAds(projectId: number): Observable<Ad[]> {
    return this.http.get<Ad[]>(`${this.baseUrl}Ads/projectid-${projectId}?api-version=${this.apiVersion}`, { headers: this.headers });
  }

  public getAd(id): Observable<Ad> {
    return this.http.get<Ad>(`${this.baseUrl}Ads/${id}?api-version=${this.apiVersion}`, { headers: this.headers });
  }

  public deleteAd(id): Observable<any> {
    return this.http.delete(`${this.baseUrl}Ads/${id}?api-version=${this.apiVersion}`, { headers: this.headers });
  }

  public getAdClickStats(id): Observable<AdClicksStats[]> {
    return this.http.get<AdClicksStats[]>(`${this.baseUrl}Ads/adclicksstats?api-version=${this.apiVersion}&adId=${id}`, { headers: this.headers });
  }

  public getAdClickStatsRange(id, from: Date, to: Date): Observable<AdClicksStats[]> {
    return this.http.get<AdClicksStats[]>(`${this.baseUrl}Ads/adclicksstatsrange?api-version=${this.apiVersion}&adId=${id}&from=${from.toUTCString()}&to=${to.toUTCString()}`, { headers: this.headers });
  }

  public getAdShowStats(id): Observable<AdShowStats[]> {
    return this.http.get<AdShowStats[]>(`${this.baseUrl}Ads/adshowstats?api-version=${this.apiVersion}&adId=${id}`, { headers: this.headers });
  }
  
  public getAdShowStatsRange(id, from: Date, to: Date): Observable<AdShowStats[]> {
    return this.http.get<AdShowStats[]>(`${this.baseUrl}Ads/adshowstatsrange?api-version=${this.apiVersion}&adId=${id}&from=${from.toUTCString()}&to=${to.toUTCString()}`, { headers: this.headers });
  }
  public createAd(id): Observable<Ad> {
    return this.http.get<Ad>(`${this.baseUrl}Ads/Create/${id}?api-version=${this.apiVersion}`, { headers: this.headers });
  }

  public putAd(ad: Ad): Observable<any> {
    return this.http.put<Ad>(`${this.baseUrl}Ads/${ad.id}?api-version=${this.apiVersion}`, ad, { headers: this.headers });
  }

  public getFile(id): Observable<FileM> {
    return this.http.get<FileM>(`${this.baseUrl}Files/${id}?api-version=${this.apiVersion}`, { headers: this.headers });
  }

  public uploadFile(userId, files): Observable<any> {
    if (files.length === 0) {
      return;
    }
    const fileToUpload = files[0] as File;
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    let tempHeaders = new HttpHeaders().set('Authorization', 'Bearer ' + this.token.accessToken);
    return this.http.post(`${this.baseUrl}Files/uploadfile?api-version=${this.apiVersion}&userId=${userId}`, formData, { headers: tempHeaders, reportProgress: true, observe: 'events' });
  }

  public deleteFile(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}Files/${id}?api-version=${this.apiVersion}`, { headers: this.headers });
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
