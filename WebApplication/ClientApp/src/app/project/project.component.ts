import { Component } from '@angular/core';
import { CrosspService } from '../services/crossp.service';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
})

export class ProjectComponent {

  constructor(protected crosspService: CrosspService) {
  }
}
