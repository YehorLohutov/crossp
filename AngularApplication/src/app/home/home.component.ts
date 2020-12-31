import { Component, OnInit } from '@angular/core';
import {Project} from '../models/project';
import {CrosspService} from '../services/crossp.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  public projects: Project[] = null;

  constructor(protected crosspService: CrosspService) {
  }

  ngOnInit(): void {
    this.crosspService.getProjects().subscribe(result => { this.projects = result; });
  }

  public createProject(): void {
    this.crosspService.createProject().subscribe(result => this.projects.push(result));
  }

  public projectsLoaded(): boolean { return this.projects != null; }
}
