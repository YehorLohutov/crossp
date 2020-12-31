import { Component, OnInit } from '@angular/core';
import { CrosspService } from '../services/crossp.service';
import { FileM } from '../models/file';
import {HttpEventType} from '@angular/common/http';

@Component({
  selector: 'app-storage',
  templateUrl: './storage.component.html',
  styleUrls: ['./storage.component.css']
})
export class StorageComponent implements OnInit {
  public files: FileM[] = null;

  public uploadingImg: boolean;
  public uploadingImgProgress;

  constructor(protected crosspService: CrosspService) {
  }

  ngOnInit(): void {
    this.loadFiles();
  }

  private loadFiles(): void {
    this.files = null;
    this.crosspService.getFiles(this.crosspService.getUserLogin()).subscribe(result => { this.files = result; });
  }

  public uploadFile = (files) => {
    this.uploadingImg = true;
    this.crosspService.uploadFile(this.crosspService.getUserLogin(), files).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.uploadingImgProgress = Math.round(100 * event.loaded / event.total);
      }
      else if (event.type === HttpEventType.Response) {
        this.loadFiles();
      }
    });
  }

  public deleteFile(fileId: number): void {
    this.crosspService.deleteFile(fileId).subscribe(res =>  this.loadFiles());
  }

  public filesLoaded(): boolean { return this.files != null; }
}
