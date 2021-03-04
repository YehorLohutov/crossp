import {Component, Input, OnInit} from '@angular/core';
import { Ad } from '../models/ad';
import {ActivatedRoute, Router} from '@angular/router';
import {CrosspService} from '../services/crossp.service';
import {switchMap} from 'rxjs/operators';
import {HttpEventType} from '@angular/common/http';
import { FileM } from '../models/file';
import { stringify } from '@angular/compiler/src/util';
import { EChartsOption } from 'echarts';
import { getLocaleDateTimeFormat } from '@angular/common';
import bulmaCalendar from 'bulma-calendar/dist/js/bulma-calendar.min.js';
@Component({
  selector: 'app-ad',
  templateUrl: './ad.component.html',
  styleUrls: ['./ad.component.css']
})
export class AdComponent implements OnInit {

  public ad: Ad;
  public file: FileM;
  public possibleAdFiles: FileM[];

  public chartOption: EChartsOption;
  //  = {
  //   xAxis: {
  //     type: 'value',
  //     data: [1, 2, 3, 4, 5, 6, 7],
  //   },
  //   yAxis: {
  //     type: 'value',
  //   },
  //   series: [
  //     {
  //       data: [1, 2, 3, 4, 5, 6, 7],
  //       type: 'line',
  //     },
  //   ],
  // };



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
          this.crosspService.getAdClickStats(this.ad.id).subscribe(res => { 
            //console.log(res); 
            
            //var ad = new Date(res[0].date);
            //console.log(ad);

            let numbers = res.map(ds => ds.number);
            console.log(numbers);

            let date = res.map(ds => new Date(ds.date).toDateString());
            console.log(date);
            this.chartOption = {
              xAxis: {
                type: 'category',
                data: date,
              },
              yAxis: {
                type: 'value',
              },
              series: [
                {
                  data: numbers,
                  type: 'line',
                },
              ],
              dataZoom: [
                {
                  type: 'inside',
                },
              ],
            };
          } );
          // this.adImgSrc = this.crosspService.getAdImgSrc(this.ad);
        }));
    this.crosspService.getFiles(this.crosspService.getUserId()).subscribe(res => this.possibleAdFiles = res);
  }

  ngOnInit(): void {
  }

  public putAd() {
    this.ad.fileId = this.file.id.toString();
    this.crosspService.putAd(this.ad).subscribe(res => {
      console.log(res);
    });
  }

  public adLoaded(): boolean { return this.ad != null; }
  public fileLoaded(): boolean { return this.file != null; }
  public possibleAdFilesLoaded(): boolean { return this.possibleAdFiles != null; }
}
