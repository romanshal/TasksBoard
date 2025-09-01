import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { catchError, map, Observable, throwError } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BoardModel } from '../../models/board/board.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { ResultResponse } from '../../models/response/response.model';
import { PaginatedList } from '../../models/paginated-list/paginated-list.model';
import { BoardForViewModel } from '../../models/board/board-for-view.model';
import { BoardAccessRequestModel } from '../../models/board-access-request/board-access-request.model';
import { BoardInviteRequestModel } from '../../models/board-invite-request/board-invite-request.model';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardMemberPermission } from '../../models/board-member-permission/board-member-permission.model';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  private BOARD_URL: string = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService
  ) { }

  getBoardsByUserId(query: string, userId: string, pageIndex: number, pageSize: number): Observable<PaginatedList<BoardForViewModel>> {
    const url = '/api/boards/user/' + userId;
    return this.http.get(this.BOARD_URL + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize).append('query', query) })
      .pipe(
        map((response: any) => {
          const paginatedList = new PaginatedList<BoardForViewModel>();
          if (response.result?.items) {
            paginatedList.Items = response.result.items.map((item: any) => {
              let board = new BoardForViewModel(
                item.id,
                item.name,
                item.description,
                item.tags,
                item.memberCount,
                item.isMember,
                item.public,
                item.image,
                item.imageExtension
              );

              return board;
            });
          }

          paginatedList.TotalCount = response.result.totalCount;
          paginatedList.PageIndex = response.result.pageIndex;
          paginatedList.PageSize = response.result.pageSize;
          paginatedList.PagesCount = response.result.pagesCount

          return paginatedList;
        }),
        catchError((error) => {
          // Handle the error here
          console.error("Get board by user id failed", error);
          return throwError(() => new Error("Get board by user id, please try again later."));
        })
      );
  }

  getBoardById(boardId: any): Observable<BoardModel> {
    const url = '/api/boards/' + boardId;
    return this.http.get<ResultResponse<BoardModel>>(this.BOARD_URL + url)
      .pipe(
        map((response: any) => {
          let board = new BoardModel();
          board.Id = response.result.id;
          board.OwnerId = response.result.ownerId;
          board.Name = response.result.name;
          board.Description = response.result.description;
          board.Tags = response.result.tags;
          board.Public = response.result.public;
          board.Image = response.result.image;
          board.ImageExtension = response.result.imageExtension;
          board.AccessRequests = response.result.accessRequests.map((item: any) => {
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
          board.InviteRequests = response.result.inviteRequests.map((item: any) => {
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
          board.Members = response.result.members.map((item: any) => {
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

          return board;
        }),
        catchError((error) => {
          // Handle the error here
          console.error("Get board by id failed", error);
          return throwError(() => new Error("Get board by id, please try again later."));
        })
      );
  }

  getPublicBoards(pageIndex: number, pageSize: number): Observable<PaginatedList<BoardForViewModel>> {
    const url = '/api/boards/public';
    return this.http.get(this.BOARD_URL + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize) })
      .pipe(
        map((response: any) => {
          const paginatedList = new PaginatedList<BoardForViewModel>();
          if (response.result?.items) {
            paginatedList.Items = response.result.items.map((item: any) => {
              let board = new BoardForViewModel(
                item.id,
                item.name,
                item.description,
                item.tags,
                item.memberCount,
                item.isMember,
                item.public,
                item.image,
                item.imageExtension
              );

              return board;
            });
          }

          paginatedList.TotalCount = response.result.totalCount;
          paginatedList.PageIndex = response.result.pageIndex;
          paginatedList.PageSize = response.result.pageSize;
          paginatedList.PagesCount = response.result.pagesCount

          return paginatedList;
        }),
        catchError((error) => {
          // Handle the error here
          console.error("Get board by user id failed", error);
          return throwError(() => new Error("Get board by user id, please try again later."));
        })
      );
  }

  createBoard(form: any): Observable<string> {
    const url = '/api/boards/';
    return this.http.post<ResultResponse<string>>(this.BOARD_URL + url, form)
      .pipe(
        map((response: any) => {
          return response.result;
        })
      );
  }
}
