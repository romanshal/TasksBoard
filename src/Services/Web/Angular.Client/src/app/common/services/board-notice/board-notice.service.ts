import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { BoardNoticeModel } from '../../models/board-notice/board-notice.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { ResultResponse, Response, unwrapResponse } from '../../models/response/response.model';
import { PaginatedList } from '../../models/paginated-list/paginated-list.model';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class BoardNoticeService {
  private BASE_URL = environment.gatewayUrl;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService
  ) { }

  getBoardNoticeById(boardId: string, boardNoticeId: string): Observable<BoardNoticeModel> {
    const url = '/notices/board/' + boardId + '/notice/' + boardNoticeId;
    return this.http
      .get<ResultResponse<BoardNoticeModel>>(this.BASE_URL + url)
      .pipe(unwrapResponse<BoardNoticeModel>()) as Observable<BoardNoticeModel>;
  }

  getBoardNoticesByBoardId(boardId: any, pageIndex: number, pageSize: number): Observable<PaginatedList<BoardNoticeModel>> {
    const url = '/notices/board/' + boardId;
    return this.http
      .get<ResultResponse<PaginatedList<BoardNoticeModel>>>(this.BASE_URL + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize) })
      .pipe(unwrapResponse<PaginatedList<BoardNoticeModel>>()) as Observable<PaginatedList<BoardNoticeModel>>;
  }

  createBoardNotice(boardId: any, notice: FormGroup): Observable<string> {
    const url = '/notices/board/' + boardId;
    return this.http
      .post<ResultResponse<string>>(this.BASE_URL + url, notice)
      .pipe(unwrapResponse<string>()) as Observable<string>;
  }

  updateBoardNotice(boardId: any, notice: FormGroup) {
    const url = '/notices/board/' + boardId;
    return this.http.put<ResultResponse<string>>(this.BASE_URL + url, notice);
  }

  updateBoardNoticeStatus(boardId: any, form: any): Observable<string> {
    const url = '/notices/status/board/' + boardId;
    return this.http
      .put<ResultResponse<string>>(this.BASE_URL + url, form)
      .pipe(unwrapResponse<string>()) as Observable<string>;
  }

  deleteBoardNotice(boardId: any, noticeId: string): Observable<void> {
    const url = '/notices/board/' + boardId + '/notice/' + noticeId;
    return this.http
      .delete<Response>(this.BASE_URL + url)
      .pipe(unwrapResponse<void>());
  }
}
