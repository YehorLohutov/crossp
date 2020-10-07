import { Component } from '@angular/core';
import { CrosspService } from '../services/crossp.service';
import { ActivatedRoute } from '@angular/router';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
})

export class ProjectComponent {
  protected project: Project;
  constructor(protected route: ActivatedRoute, protected crosspService: CrosspService) {
    this.route.paramMap
      .pipe(
        switchMap(params => params.get('id')))
      .subscribe(id => this.crosspService.getProject(id).subscribe(result => this.project = result));
  }
}
