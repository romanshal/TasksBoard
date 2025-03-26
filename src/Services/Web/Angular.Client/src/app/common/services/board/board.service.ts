import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { catchError, map, Observable, throwError } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BoardModel } from '../../models/board/board.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { ResultResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  private BOARD_URL: string = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService
  ) { }

  getBoardsByUserId(userId: string, pageIndex: number, pageSize: number): Observable<BoardModel[]> {
    const url = '/api/boards/user/' + userId;
    return this.http.get<ResultResponse<BoardModel[]>>(this.BOARD_URL + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize) })
      .pipe(
        map((response: any) => {
          return response.result.map((board: any) => {
            return new BoardModel(board.ownerId, board.name, board.description);
          })
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
          return new BoardModel(response.result.ownerId, response.result.name, response.result.description)
        }),
        catchError((error) => {
          // Handle the error here
          console.error("Get board by id failed", error);
          return throwError(() => new Error("Get board by id, please try again later."));
        })
      );
  }
}
