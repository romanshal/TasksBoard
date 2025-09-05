import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BoardInviteRequestModel } from '../../models/board-invite-request/board-invite-request.model';
import { ResultResponse, unwrapResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class BoardInviteRequestService {
  private BOARD_INVITE_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient
  ) { }

  getByAccountId(accountId: string): Observable<BoardInviteRequestModel[]> {
    const url = '/inviterequests/account/' + accountId;
    return this.http
      .get<ResultResponse<BoardInviteRequestModel[]>>(this.BOARD_INVITE_URL + url)
      .pipe(unwrapResponse<BoardInviteRequestModel[]>()) as Observable<BoardInviteRequestModel[]>;
  }

  resolveBoardInviteRequest(boardId: string, resolve: any): Observable<string> {
    const url = '/ainviterequests/resolve/board/' + boardId;
    return this.http
      .post<ResultResponse<string>>(this.BOARD_INVITE_URL + url, resolve)
      .pipe(unwrapResponse<string>()) as Observable<string>;
  }
}
