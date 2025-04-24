import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { catchError, map, Observable, throwError } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BoardModel } from '../../models/board/board.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { ResultResponse } from '../../models/response/response.model';
import { PaginatedList } from '../../models/paginated-list/paginated-list.model';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  private BOARD_URL: string = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService
  ) { }

  getBoardsByUserId(userId: string, pageIndex: number, pageSize: number): Observable<PaginatedList<BoardModel>> {
    const url = '/api/boards/user/' + userId;
    return this.http.get<ResultResponse<BoardModel[]>>(this.BOARD_URL + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize) })
      .pipe(
        map((response: any) => {
          const paginatedList = new PaginatedList<BoardModel>();
          if (response.result?.items) {
            paginatedList.Items = response.result.items.map((item: any) => {
              let board = new BoardModel(
                item.id,
                item.ownerId,
                item.name,
                item.description,
                item.tags
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
          return new BoardModel(
            response.result.id,
            response.result.ownerId,
            response.result.name,
            response.result.description,
            response.result.tags)
        }),
        catchError((error) => {
          // Handle the error here
          console.error("Get board by id failed", error);
          return throwError(() => new Error("Get board by id, please try again later."));
        })
      );
  }
}
