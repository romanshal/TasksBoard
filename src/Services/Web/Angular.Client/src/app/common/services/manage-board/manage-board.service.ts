import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Response, ResultResponse, unwrapResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class ManageBoardService {
  private BOARD_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient,
  ) { }

  updateBoard(boardId: string, board: any): Observable<string> {
    const url = '/boards/' + boardId;
    return this.http
      .put<ResultResponse<string>>(this.BOARD_URL + url, board)
      .pipe(unwrapResponse<string>()) as Observable<string>;
  }

  deleteBoard(boardId: string): Observable<void> {
    const url = '/boards/' + boardId;
    return this.http
      .delete<Response>(this.BOARD_URL + url)
      .pipe(unwrapResponse<void>());
  }
}
