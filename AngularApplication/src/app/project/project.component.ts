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

  public project: Project = null;
  public ads: Ad[] = null;

  options = {
    title: {
      text: 'Ads clicks',
      x: 'center'
    },
    tooltip: {
      trigger: 'item',
      formatter: '{a} <br/>{b} : {c} ({d}%)'
    },
    legend: {
      x: 'center',
      y: 'bottom',
      data: ['rose1', 'rose2', 'rose3', 'rose4', 'rose5', 'rose6', 'rose7', 'rose8']
    },
    calculable: true,
    series: [
      {
        name: 'area',
        type: 'pie',
        radius: [30, 110],
        roseType: 'area',
        data: [
          { value: 10, name: 'rose1' },
          { value: 5, name: 'rose2' },
          { value: 15, name: 'rose3' },
          { value: 25, name: 'rose4' },
          { value: 20, name: 'rose5' },
          { value: 35, name: 'rose6' },
          { value: 30, name: 'rose7' },
          { value: 40, name: 'rose8' }
        ]
      }
    ]
  };



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
              this.ads = res, error3 => console.log(error3));
        }, error2 => console.log(error2)), error1 => console.log(error1));
  }
  ngOnInit(): void {

  }
  public deleteProject(): void {
    this.crosspService.deleteProject(this.project.id).subscribe(result => this.router.navigate(['']));
  }
  public putProject(): void {
    this.crosspService.putProject(this.project).subscribe(result => console.log(result));
  }
  public createAd(): void {
    this.crosspService.createAd(this.project.id).subscribe(result => this.ads.push(result));
  }

  public projectLoaded(): boolean { return this.project != null; }
  public adsLoaded(): boolean { return this.ads != null; }
}
