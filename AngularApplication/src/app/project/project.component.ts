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

  public project: Project;
  public ad: Ad;
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
              this.ad = res[0], error3 => console.log(error3));
        }, error2 => console.log(error2)), error1 => console.log(error1));
  }

  ngOnInit(): void {
    }
  public deleteProject() {
    this.crosspService.deleteProject(this.project.id).subscribe(result => this.router.navigate(['']));
  }
  public putProject() {
    this.crosspService.putProject(this.project);
  }
  public putAd() {
    this.crosspService.putAd(this.ad).subscribe(res => {
      console.log(res);
    });
  }
  public uploadFile = (files) => {
    this.crosspService.uploadAdImage(this.ad, files);
  }

}
