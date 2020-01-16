import { Component, OnInit } from '@angular/core';
import { ChunkSettings } from '@progress/kendo-angular-upload';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html'
})
export class UploadComponent implements OnInit {

  uploadSaveUrl = 'http://localhost/TestChunkUpload/api/File/Upload';
  uploadRemoveUrl = 'http://localhost/TestChunkUpload/api/File/Remove';

  public chunkSettings: ChunkSettings = {
    size: 102400
  };

  constructor() { }

  ngOnInit() {
  }

}
