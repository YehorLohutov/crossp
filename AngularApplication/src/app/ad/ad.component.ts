import {Component, Input, OnInit} from '@angular/core';
import { Ad } from '../models/ad';
import {ActivatedRoute, Router} from '@angular/router';
import {CrosspService} from '../services/crossp.service';
import {switchMap} from 'rxjs/operators';
import {HttpEventType} from '@angular/common/http';
import { FileM } from '../models/file';

@Component({
  selector: 'app-ad',
  templateUrl: './ad.component.html',
  styleUrls: ['./ad.component.css']
})
export class AdComponent implements OnInit {

  public ad: Ad;
  public file: FileM;
  public possibleAdFiles: FileM[];

  constructor(protected route: ActivatedRoute,
              public crosspService: CrosspService,
              protected router: Router) {
    this.route.paramMap
      .pipe(
        switchMap(params => params.get('id')))
      .subscribe(id => this.crosspService.getAd(id)
        .subscribe(result => {
          this.ad = result;
          this.crosspService.getFile(this.ad.fileId).subscribe(adFile => this.file = adFile );
          // this.adImgSrc = this.crosspService.getAdImgSrc(this.ad);
        }));
    this.crosspService.getFiles(this.crosspService.getUserId()).subscribe(res => this.possibleAdFiles = res);
  }

  ngOnInit(): void {
  }

  public putAd() {
    this.crosspService.putAd(this.ad).subscribe(res => {
      console.log(res);
    });
  }
}
