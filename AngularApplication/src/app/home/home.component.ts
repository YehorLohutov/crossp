import { Component, OnInit } from '@angular/core';
import {Project} from '../models/project';
import {CrosspService} from '../services/crossp.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  public projectsLoaded: boolean;
  public projects: Project[];


  constructor(protected crosspService: CrosspService) {
    this.projectsLoaded = false;
  }

  ngOnInit(): void {
    this.crosspService.getProjects().subscribe(result => { this.projects = result; this.projectsLoaded = true; });
  }

  public createProject(): void {
    this.crosspService.createProject().subscribe(result => this.projects.push(result));
  }
}
