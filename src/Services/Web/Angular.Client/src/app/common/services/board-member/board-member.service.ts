import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { ResultResponse, unwrapResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class BoardMemberService {
  private BOARD_MEMBER_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient
  ) { }

  getBoardMembersByBoardId(boardId: any): Observable<BoardMemberModel[]> {
    const url = '/members/board/' + boardId;
    return this.http
      .get<ResultResponse<BoardMemberModel[]>>(this.BOARD_MEMBER_URL + url)
      .pipe(unwrapResponse<BoardMemberModel[]>()) as Observable<BoardMemberModel[]>;
  }
}
