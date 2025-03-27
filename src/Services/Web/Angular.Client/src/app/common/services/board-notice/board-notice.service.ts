import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { catchError, map, Observable, throwError } from 'rxjs';
import { BoardNoticeModel } from '../../models/board-notice/board-notice.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { ResultResponse } from '../../models/response/response.model';
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

  getBoardNoticesByBoardId(boardId: any, pageIndex: number, pageSize: number): Observable<PaginatedList<BoardNoticeModel>> {
    const url = '/api/boardnotices/board/' + boardId;
    return this.http.get<ResultResponse<PaginatedList<BoardNoticeModel>>>(this.BASE_URL + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize) })
      .pipe(
        map((response: any) => {
          const paginatedList = new PaginatedList<BoardNoticeModel>();
          if (response.result?.items) {
            paginatedList.Items = response.result.items.map((item: { authorId: string; boardId: string; boardName: string; definition: string; noticeStatusName: string; }) => new BoardNoticeModel(
              item.authorId,
              item.boardId,
              item.boardName,
              item.definition,
              item.noticeStatusName
            ));
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
}
