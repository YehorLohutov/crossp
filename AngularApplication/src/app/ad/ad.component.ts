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
import { from } from 'rxjs';
@Component({
  selector: 'app-ad',
  templateUrl: './ad.component.html',
  styleUrls: ['./ad.component.css']
})
export class AdComponent implements OnInit {

  public ad: Ad;
  public file: FileM;
  public possibleAdFiles: FileM[];

  public chartClicksOption = {
    title: {
      text: 'Clicks',
    },
    xAxis: {
      type: 'category',
      data: [ new Date().toDateString() ],
    },
    yAxis: {
      type: 'value',
    },
    series: [
      {        
        type: 'line',
        data: [0],
      },
    ],
    dataZoom: [
      {
        type: 'inside',
      },
    ],
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'line',
        label: {
          backgroundColor: '#6a7985'
        }
      }
    },
  };

  public chartShowOption = {
    title: {
      text: 'Show',
    },
    xAxis: {
      type: 'category',
      data: [ new Date().toDateString() ],
    },
    yAxis: {
      type: 'value',
    },
    series: [
      {        
        type: 'line',
        data: [0],
      },
    ],
    dataZoom: [
      {
        type: 'inside',
      },
    ],
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'line',
        label: {
          backgroundColor: '#6a7985'
        }
      }
    },
  };

  public dateFrom: any;
  public dateTo: any;


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

  public deleteAd() {
    this.crosspService.deleteAd(this.ad.id).subscribe(res => {
      console.log(res);
      this.router.navigate(['/project', this.ad.projectId]);
    });
  }

  public adLoaded(): boolean { return this.ad != null; }
  public fileLoaded(): boolean { return this.file != null; }
  public possibleAdFilesLoaded(): boolean { return this.possibleAdFiles != null; }

  public updateAnalytics() {
    const tempDateFrom = new Date(this.dateFrom);
    const tempDateTo = new Date(this.dateTo);

    console.log(tempDateFrom.toUTCString() + ' - ' + tempDateTo.toUTCString());
    this.updateChartClicks(tempDateFrom, tempDateTo);
    this.updateChartShow(tempDateFrom, tempDateTo);
  }

  private updateChartClicks(dateFrom: Date, dateTo: Date) {
    this.crosspService.getAdClickStatsRange(this.ad.id, dateFrom, dateTo).subscribe(res => {
      let numbers = res.map(ds => ds.number);
      console.log(numbers);

      let date = res.map(ds => new Date(ds.date).toDateString());
      console.log(date);

      this.chartClicksOption = {
        title: {
          text: 'Clicks',
        },
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
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'line',
            label: {
              backgroundColor: '#6a7985'
            }
          }
        },
      };
    });
  }

  private updateChartShow(dateFrom: Date, dateTo: Date) {
    this.crosspService.getAdShowStatsRange(this.ad.id, dateFrom, dateTo).subscribe(res => {
      let numbers = res.map(ds => ds.number);
      console.log(numbers);

      let date = res.map(ds => new Date(ds.date).toDateString());
      console.log(date);

      this.chartShowOption = {
        title: {
          text: 'Show',
        },
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
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'line',
            label: {
              backgroundColor: '#6a7985'
            }
          }
        },
      };
    });
  }


}
