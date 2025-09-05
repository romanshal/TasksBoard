import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { BoardModel } from '../../models/board/board.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { ResultResponse, unwrapResponse } from '../../models/response/response.model';
import { PaginatedList } from '../../models/paginated-list/paginated-list.model';
import { BoardForViewModel } from '../../models/board/board-for-view.model';


@Injectable({
  providedIn: 'root'
})
export class BoardService {
  private BOARD_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService
  ) { }

  getBoardsByUserId(query: string, userId: string, pageIndex: number, pageSize: number): Observable<PaginatedList<BoardForViewModel>> {
    const url = '/boards/user/' + userId;
    return this.http
      .get<ResultResponse<PaginatedList<BoardForViewModel>>>(this.BOARD_URL + url, {
        params: this.httpOption.getPaginationOptions(pageIndex, pageSize).append('query', query)
      })
      .pipe(unwrapResponse<PaginatedList<BoardForViewModel>>()) as Observable<PaginatedList<BoardForViewModel>>;
  }

  getBoardById(boardId: string): Observable<BoardModel> {
    const url = '/boards/' + boardId;
    return this.http
      .get<ResultResponse<BoardModel>>(this.BOARD_URL + url)
      .pipe(unwrapResponse<BoardModel>()) as Observable<BoardModel>;
  }

  getPublicBoards(pageIndex: number, pageSize: number): Observable<PaginatedList<BoardForViewModel>> {
    const url = '/boards/public';
    return this.http
      .get<ResultResponse<PaginatedList<BoardForViewModel>>>(this.BOARD_URL + url, {
        params: this.httpOption.getPaginationOptions(pageIndex, pageSize)
      })
      .pipe(unwrapResponse<PaginatedList<BoardForViewModel>>()) as Observable<PaginatedList<BoardForViewModel>>;
  }

  createBoard(form: any): Observable<string> {
    const url = '/boards';
    return this.http
      .post<ResultResponse<string>>(this.BOARD_URL + url, form)
      .pipe(unwrapResponse<string>()) as Observable<string>;
  }
}
