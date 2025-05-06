import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { map, Observable } from 'rxjs';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { PaginatedList } from '../../models/paginated-list/paginated-list.model';
import { ResultResponse } from '../../models/response/response.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { BoardMemberPermission } from '../../models/board-member-permission/board-member-permission.model';

@Injectable({
  providedIn: 'root'
})
export class BoardMemberService {
  private BOARD_MEMBER_URL: string = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService
  ) { }

  getBoardMembersByBoardId(boardId: any): Observable<BoardMemberModel[]> {
    const url = '/api/boardmembers/board/' + boardId;
    return this.http.get<ResultResponse<BoardMemberModel[]>>(this.BOARD_MEMBER_URL + url)
      .pipe(
        map((response: any) => {
          let list: BoardMemberModel[] = [];
          if (response.result) {
            list = response.result.map((item: any) => {
              let member = new BoardMemberModel();
              member.Id = item.id;
              member.AccountId = item.accountId;
              member.BoardId = item.boardId;
              member.IsOwner = item.isOwner;
              member.Nickname = item.nickname;
              member.CreatedAt = item.createdAt;
              member.Permissions = item.permissions.map((perm: any) => {
                let permission = new BoardMemberPermission();
                permission.BoardMemberId = perm.boardMemberId;
                permission.BoardPermissionId = perm.boardPermissionId;
                permission.BoardPermissionName = perm.boardPermissionName;
                permission.BoardPermissionDescription = perm.boardPermissionDescription;

                return permission;
              })

              return member;
            });
          }

          return list;
        })
      );
  }
}
