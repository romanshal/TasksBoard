import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Response, ResultResponse, unwrapResponse } from '../../models/response/response.model';
import { BoardAccessRequestModel } from '../../models/board-access-request/board-access-request.model';

@Injectable({
  providedIn: 'root'
})
export class BoardAccessRequestService {
  private BOARD_ACCESS_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient
  ) { }

  requestBoardAccess(boardId: string, form: any): Observable<string> {
    const url = '/accessrequests/board/' + boardId;
    return this.http
      .post<ResultResponse<string>>(this.BOARD_ACCESS_URL + url, form)
      .pipe(unwrapResponse<string>()) as Observable<string>;
  }

  getByAccountId(accountId: string): Observable<BoardAccessRequestModel[]> {
    const url = `/accessrequests/account/${accountId}`;

    return this.http
      .get<ResultResponse<BoardAccessRequestModel[]>>(this.BOARD_ACCESS_URL + url)
      .pipe(unwrapResponse<BoardAccessRequestModel[]>()) as Observable<BoardAccessRequestModel[]>;
  }

  cancelAccessRequest(body: any) {
    const url = '/accessrequests/cancel';
    return this.http
      .post<Response>(this.BOARD_ACCESS_URL + url, body)
      .pipe(unwrapResponse<void>());
  }
}
