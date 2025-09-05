import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { BoardInviteRequestModel } from '../../models/board-invite-request/board-invite-request.model';
import { Response, ResultResponse, unwrapResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class ManageBoardInviteRequestService {
  private BOARD_INVITE_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient
  ) { }

  createInviteRequest(boardId: string, invite: any): Observable<string> {
    const url = '/inviterequests/board/' + boardId;
    return this.http
      .post<ResultResponse<string>>(this.BOARD_INVITE_URL + url, invite)
      .pipe(unwrapResponse<string>()) as Observable<string>;
  }

  getBoardInviteRequestByBoardId(boardId: string): Observable<BoardInviteRequestModel[]> {
    const url = '/inviterequests/board/' + boardId;
    return this.http
      .get<ResultResponse<BoardInviteRequestModel[]>>(this.BOARD_INVITE_URL + url)
      .pipe(unwrapResponse<BoardInviteRequestModel[]>()) as Observable<BoardInviteRequestModel[]>;
  }

  cancelInviteRequest(boardId: string, invite: any): Observable<void> {
    const url = '/inviterequests/board/' + boardId;
    return this.http
      .put<Response>(this.BOARD_INVITE_URL + url, invite)
      .pipe(unwrapResponse<void>());
  }
}
