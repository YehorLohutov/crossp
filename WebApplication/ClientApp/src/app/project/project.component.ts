import { Component } from '@angular/core';
import { CrosspService } from '../services/crossp.service';
import { ActivatedRoute, Router } from '@angular/router';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
})

export class ProjectComponent {
  protected project: Project;
  //protected ads: Ad[];
  protected ad: Ad;
  constructor(protected route: ActivatedRoute,
    protected crosspService: CrosspService,
    protected router: Router
  ) {
    this.route.paramMap
      .pipe(
        switchMap(params => params.get('id')))
      .subscribe(id => this.crosspService.getProject(id).subscribe(result => {
        this.project = result;
        this.crosspService.getAds(this.project.id).subscribe(res =>
          this.ad = res[0]);
      }));
  }
  protected deleteProject() {
    this.crosspService.deleteProject(this.project.id).subscribe(result => this.router.navigate(['']));
  }
  protected putProject() {
    this.crosspService.putProject(this.project);
  }
  protected putAd() {
    this.crosspService.putAd(this.ad).subscribe(res => {
      console.log(res);
    });
  }
  protected uploadFile = (files) => {
    this.crosspService.uploadAdImage(this.ad, files);
  }
}
