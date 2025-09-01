import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { map, Observable } from 'rxjs';
import { BoardInviteRequestModel } from '../../models/board-invite-request/board-invite-request.model';

@Injectable({
  providedIn: 'root'
})
export class ManageBoardInviteRequestService {
  private BOARD_INVITE_URL: string = environment.apiUrl;

  constructor(
    private http: HttpClient
  ) { }

  createInviteRequest(boardId: string, invite: any) {
    const url = '/api/manageinviterequests/board/' + boardId;
    return this.http.post(this.BOARD_INVITE_URL + url, invite);
  }

  getBoardInviteRequestByBoardId(boardId: string): Observable<BoardInviteRequestModel[]> {
    const url = '/api/manageinviterequests/board/' + boardId;
    return this.http.get(this.BOARD_INVITE_URL + url)
      .pipe(
        map((response: any) => {
          let list: BoardInviteRequestModel[] = [];
          if (response.result) {
            list = response.result.map((item: any) => {
              let request = new BoardInviteRequestModel(
                item.id,
                item.boardId,
                item.boardName,
                item.fromAccountId,
                item.fromAccountName,
                item.toAccountId,
                item.toAccountName,
                item.toAccountEmail,
                item.createdAt
              );

              return request;
            });
          }

          return list;
        })
      );
  }

  cancelInviteRequest(boardId: string, invite: any){
    const url = '/api/manageinviterequests/board/' + boardId;
    return this.http.put(this.BOARD_INVITE_URL + url, invite);
  }
}
