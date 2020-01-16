import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { File } from './models/file.model';
import { map } from 'rxjs/operators';

@Injectable()
export class AppService {

  constructor(private http: HttpClient) { }

  public getFileList(): Observable<File[]> {
    return this.http.get<File[]>('http://localhost/TestChunkUpload/api/File/List');
  }

  public getFile(file: string): Observable<any> {
    return this.http.get(`http://localhost/TestChunkUpload/api/File/Download/${file}`, { responseType: 'blob' });
  }
}
