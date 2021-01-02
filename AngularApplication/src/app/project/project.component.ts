import { Component, OnInit } from '@angular/core';
import {Project} from '../models/project';
import {Ad} from '../models/ad';
import {ActivatedRoute, Router} from '@angular/router';
import {CrosspService} from '../services/crossp.service';
import {switchMap} from 'rxjs/operators';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.css']
})
export class ProjectComponent implements OnInit {

  public project: Project = null;
  public ads: Ad[] = null;

  constructor(protected route: ActivatedRoute,
              protected crosspService: CrosspService,
              protected router: Router
  ) {
    this.route.paramMap
      .pipe(
        switchMap(params => params.get('id')))
      .subscribe(id => this.crosspService.getProject(id)
        .subscribe(result => {
          this.project = result;
          this.crosspService.getAds(this.project.id)
            .subscribe(res =>
              this.ads = res, error3 => console.log(error3));
        }, error2 => console.log(error2)), error1 => console.log(error1));
  }
  ngOnInit(): void {

  }
  public deleteProject(): void {
    this.crosspService.deleteProject(this.project.id).subscribe(result => this.router.navigate(['']));
  }
  public putProject(): void {
    this.crosspService.putProject(this.project).subscribe(result => console.log(result));
  }
  public createAd(): void {
    this.crosspService.createAd(this.project.id).subscribe(result => this.ads.push(result));
  }

  public projectLoaded(): boolean { return this.project != null; }
  public adsLoaded(): boolean { return this.ads != null; }
}
