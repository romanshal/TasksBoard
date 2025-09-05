import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BoardAccessRequestModel } from '../../models/board-access-request/board-access-request.model';
import { ResultResponse, unwrapResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class ManageBoardAccessRequestService {
  private BOARD_ACCESS_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient
  ) { }

  resolveBoardAccessRequest(boardId: string, resolve: any): Observable<string> {
    const url = '/accessrequests/resolve/board/' + boardId;
    return this.http
      .post<ResultResponse<string>>(this.BOARD_ACCESS_URL + url, resolve)
      .pipe(unwrapResponse<string>()) as Observable<string>;
  }

  getBoardAccessRequestByBoardId(boardId: string): Observable<BoardAccessRequestModel[]> {
    const url = '/accessrequests/board/' + boardId;
    return this.http
      .get<ResultResponse<BoardAccessRequestModel[]>>(this.BOARD_ACCESS_URL + url)
      .pipe(unwrapResponse<BoardAccessRequestModel[]>()) as Observable<BoardAccessRequestModel[]>;
  }
}
