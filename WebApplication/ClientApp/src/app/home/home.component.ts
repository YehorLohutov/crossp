import { Component } from '@angular/core';
import { CrosspService } from '../services/crossp.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  protected projects: Project[];
  constructor(protected crosspService: CrosspService) {
    crosspService.getProjects().subscribe(result => this.projects = result);
  }
}
