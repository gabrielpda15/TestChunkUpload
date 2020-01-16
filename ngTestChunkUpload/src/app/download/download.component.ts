import { Component, OnInit } from '@angular/core';
import { AppService } from '../app.service';
import { Observable } from 'rxjs';
import { File } from '../models/file.model';
import { saveAs } from '@progress/kendo-file-saver';

@Component({
  selector: 'app-download',
  templateUrl: './download.component.html'
})
export class DownloadComponent implements OnInit {

  public gridData: Observable<any[]>;

  constructor(private appservice: AppService) { }

  ngOnInit() {
    this.gridData = this.appservice.getFileList();
  }

  public clickEvent(item: File): void {
    this.appservice.getFile(item.fileName)
      .subscribe(x => saveAs(x, item.fileName));
  }

  public reloadEvent(): void {
    this.gridData = this.appservice.getFileList();
  }
}
