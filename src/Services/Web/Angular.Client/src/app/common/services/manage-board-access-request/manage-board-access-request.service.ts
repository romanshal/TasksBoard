import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ResultResponse } from '../../models/response/response.model';
import { BoardAccessRequestModel } from '../../models/board-access-request/board-access-request.model';

@Injectable({
  providedIn: 'root'
})
export class ManageBoardAccessRequestService {
  private BOARD_ACCESS_URL: string = environment.apiUrl;

  constructor(
    private http: HttpClient
  ) { }

  resolveBoardAccessRequest(boardId: string, resolve: any){
    const url ='/api/manageaccessrequests/resolve/board/' + boardId;
    return this.http.post(this.BOARD_ACCESS_URL + url, resolve);
  }

  getBoardAccessRequestByBoardId(boardId: string): Observable<BoardAccessRequestModel[]> {
    const url = '/api/manageaccessrequests/board/' + boardId;
    return this.http.get(this.BOARD_ACCESS_URL + url)
      .pipe(
        map((response: any) => {
          let list: BoardAccessRequestModel[] = [];
          if (response.result) {
            list = response.result.map((item: any) => {
              let request = new BoardAccessRequestModel(
                item.id,
                item.boardId,
                item.boardName,
                item.accountId,
                item.accountName,
                item.accountEmail,
                item.createdAt
              );

              return request;
            });
          }

          return list;
        })
      );
  }
}
