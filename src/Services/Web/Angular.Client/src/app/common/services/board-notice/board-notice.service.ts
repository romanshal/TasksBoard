import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { catchError, map, Observable, throwError } from 'rxjs';
import { BoardNoticeModel } from '../../models/board-notice/board-notice.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { ResultResponse, Response } from '../../models/response/response.model';
import { PaginatedList } from '../../models/paginated-list/paginated-list.model';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class BoardNoticeService {
  private BASE_URL = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService
  ) { }

  getBoardNoticeById(boardId: string, boardNoticeId: string): Observable<BoardNoticeModel> {
    const url = '/api/boardnotices/board/' + boardId + '/notice/' + boardNoticeId;
    return this.http.get(this.BASE_URL + url)
      .pipe(
        map((response: any) => {
          let note = new BoardNoticeModel();
          note.Id = response.result.id;
          note.AuthorId = response.result.authorId;
          note.AuthorName = response.result.authorName;
          note.BoardId = response.result.boardId;
          note.BoardName = response.result.boardName;
          note.Definition = response.result.definition;
          note.BackgroundColor = response.result.backgroundColor;
          note.Rotation = response.result.rotation;
          note.Completed = response.result.completed;
          note.CreatedAt = response.result.createdAt;

          return note;
        })
      );
  }

  getBoardNoticesByBoardId(boardId: any, pageIndex: number, pageSize: number): Observable<PaginatedList<BoardNoticeModel>> {
    const url = '/api/boardnotices/board/' + boardId;
    return this.http.get<ResultResponse<PaginatedList<BoardNoticeModel>>>(this.BASE_URL + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize) })
      .pipe(
        map((response: any) => {
          const paginatedList = new PaginatedList<BoardNoticeModel>();
          if (response.result?.items) {
            paginatedList.Items = response.result.items.map((item: any) => {
              let note = new BoardNoticeModel();
              note.Id = item.id;
              note.AuthorId = item.authorId;
              note.AuthorName = item.authorName;
              note.BoardId = item.boardId;
              note.BoardName = item.boardName;
              note.Definition = item.definition;
              note.BackgroundColor = item.backgroundColor;
              note.Rotation = item.rotation;
              note.Completed = item.completed;
              note.CreatedAt = item.createdAt;

              return note;
            });
          }

          paginatedList.TotalCount = response.result.totalCount;
          paginatedList.PageIndex = response.result.pageIndex;
          paginatedList.PageSize = response.result.pageSize;
          paginatedList.PagesCount = response.result.pagesCount

          return paginatedList;
        })
      );
  }

  createBoardNotice(boardId: any, notice: FormGroup) {
    const url = '/api/managenotices/board/' + boardId;
    return this.http.post<ResultResponse<string>>(this.BASE_URL + url, notice);
  }

  updateBoardNotice(boardId: any, notice: FormGroup) {
    const url = '/api/managenotices/board/' + boardId;
    return this.http.put<ResultResponse<string>>(this.BASE_URL + url, notice);
  }

  updateBoardNoticeStatus(boardId: any, form: any) {
    const url = '/api/managenotices/status/board/' + boardId;
    return this.http.put<ResultResponse<string>>(this.BASE_URL + url, form);
  }

  deleteBoardNotice(boardId: any, noticeId: string) {
    const url = '/api/managenotices/board/' + boardId + '/notice/' + noticeId;
    return this.http.delete<Response>(this.BASE_URL + url);
  }
}
