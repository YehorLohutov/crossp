import {Component, Input, OnInit} from '@angular/core';
import {Ad} from "../models/ad";
import {ActivatedRoute, Router} from "@angular/router";
import {CrosspService} from "../services/crossp.service";
import {switchMap} from "rxjs/operators";
import {HttpEventType} from '@angular/common/http';
import { FileM } from "../models/file";

@Component({
  selector: 'app-ad',
  templateUrl: './ad.component.html',
  styleUrls: ['./ad.component.css']
})
export class AdComponent implements OnInit {

  public ad: Ad;
  public uploadingImg: boolean;
  public uploadingImgProgress;
  public adImgSrc;
  public possibleAdFiles: FileM[];

  constructor(protected route: ActivatedRoute,
              protected crosspService: CrosspService,
              protected router: Router) {
    this.route.paramMap
      .pipe(
        switchMap(params => params.get('id')))
      .subscribe(id => this.crosspService.getAd(id)
        .subscribe(result => {
          this.ad = result;
          this.adImgSrc = this.crosspService.getAdImgSrc(this.ad);
        }));
    this.crosspService.getFiles(this.crosspService.getUserLogin()).subscribe(res => this.possibleAdFiles = res);
  }

  ngOnInit(): void {
    this.uploadingImg = false;
  }

  public putAd() {
    this.crosspService.putAd(this.ad).subscribe(res => {
      console.log(res);
    });
  }
  public uploadFile = (files) => {
    this.uploadingImg = true;
    this.crosspService.uploadAdImage(this.ad, files).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.uploadingImgProgress = Math.round(100 * event.loaded / event.total);
      }
      else if (event.type === HttpEventType.Response) {
        this.uploadingImg = false;

        this.crosspService.getAd(this.ad.id)
          .subscribe(result => {
            this.ad = result;
            this.adImgSrc = this.crosspService.getAdImgSrc(this.ad);
          });
      }
    });
  }

}
