import { Component } from '@angular/core';
import { CrosspService } from '../services/crossp.service';
import { ActivatedRoute, Router } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { error } from 'protractor';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
})

export class ProjectComponent {
  public project: Project;
  //protected ads: Ad[];
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
