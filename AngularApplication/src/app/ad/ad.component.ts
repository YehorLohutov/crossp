import {Component, Input, OnInit} from '@angular/core';
import {Ad} from "../models/ad";
import {ActivatedRoute, Router} from "@angular/router";
import {CrosspService} from "../services/crossp.service";
import {switchMap} from "rxjs/operators";

@Component({
  selector: 'app-ad',
  templateUrl: './ad.component.html',
  styleUrls: ['./ad.component.css']
})
export class AdComponent implements OnInit {

  public ad: Ad;

  constructor(protected route: ActivatedRoute,
              protected crosspService: CrosspService,
              protected router: Router) {
    this.route.paramMap
      .pipe(
        switchMap(params => params.get('id')))
      .subscribe(id => this.crosspService.getAd(id)
        .subscribe(result => {
          this.ad = result;
        }));
  }

  ngOnInit(): void {
  }

  public putAd() {
    this.crosspService.putAd(this.ad).subscribe(res => {
      console.log(res);
    });
  }
  public uploadFile = (files) => {
    this.crosspService.uploadAdImage(this.ad, files);
  }

}
