import { Component } from '@angular/core';
import { CrosspService } from '../services/crossp.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {

  protected projectsLoaded: boolean;
  protected projects: Project[];

  constructor(protected crosspService: CrosspService) {
    this.projectsLoaded = false;
    crosspService.getProjects().subscribe(result => { this.projects = result; this.projectsLoaded = true; });
  }

  public createProject() {
    this.crosspService.addProject(new Project(10, '10')).subscribe(result => this.projects.push(result));
  }
}
