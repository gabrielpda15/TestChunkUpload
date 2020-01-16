export class File {

  public fileID: number;
  public fileName: string;

  constructor(obj: any) {
    this.fileID = obj && obj.fileID || null;
    this.fileName = obj && obj.fileName || null;
  }

}
