import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { catchError, map, Observable, throwError } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BoardModel } from '../../models/board/board.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { ResultResponse } from '../../models/response/response.model';
import { PaginatedList } from '../../models/paginated-list/paginated-list.model';
import { BoardForViewModel } from '../../models/board/board-for-view.model';

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

          return board;
        }),
        catchError((error) => {
          // Handle the error here
          console.error("Get board by id failed", error);
          return throwError(() => new Error("Get board by id, please try again later."));
        })
      );
  }

  getPublicBoards(pageIndex: number, pageSize: number) : Observable<PaginatedList<BoardForViewModel>> {
    const url = '/api/boards/public';
    return this.http.get(this.BOARD_URL + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize)})
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
