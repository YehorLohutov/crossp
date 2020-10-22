import { Component } from '@angular/core';
import { CrosspService } from '../services/crossp.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {

  public projectsLoaded: boolean;
  public projects: Project[];


  constructor(protected crosspService: CrosspService) {
    this.projectsLoaded = false;
    crosspService.getProjects().subscribe(result => { this.projects = result; this.projectsLoaded = true; });
    //this.crosspService.debug().subscribe(result => {
    //  console.log(result);
    //});

  }

  public createProject() {
    this.crosspService.createProject().subscribe(result => this.projects.push(result));
    //this.crosspService.debug();
  }
}