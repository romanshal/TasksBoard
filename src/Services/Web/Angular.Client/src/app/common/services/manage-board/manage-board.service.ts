import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ManageBoardService {
  private BOARD_URL: string = environment.apiUrl;

  constructor(
    private http: HttpClient,
  ) { }

  updateBoard(boardId: string, board: any) {
    const url = '/api/manageboards/' + boardId;
    return this.http.put(this.BOARD_URL + url, board);
  }
}
