import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { HttpOptionService } from '../http-option/http-options.service';
import { Observable } from 'rxjs';
import { BoardMessageModel } from '../../models/board-message/board-message.model';
import { ResultResponse, unwrapResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class BoardMessageService {
  private BOARD_MESSAGE_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService
  ) { }

  getMessages(boardId: string, pageIndex: number, pageSize: number): Observable<BoardMessageModel[]> {
    const url = '/messages/' + boardId;
    return this.http
      .get<ResultResponse<BoardMessageModel[]>>(this.BOARD_MESSAGE_URL + url, {
        params: this.httpOption.getPaginationOptions(pageIndex, pageSize)
      })
      .pipe(unwrapResponse<BoardMessageModel[]>()) as Observable<BoardMessageModel[]>;
  }

  sendMessage(boardId: string, form: any) {
    const url = '/messages/' + boardId;
    return this.http.post(this.BOARD_MESSAGE_URL + url, form);
  }

  editMessage(boardId: string, form: any) {
    const url = '/messages/' + boardId;
    return this.http.put(this.BOARD_MESSAGE_URL + url, form);
  }

  deleteMessage(boardId: string, messageId: string) {
    const url = '/messages/' + boardId + '/' + messageId;
    return this.http.delete(this.BOARD_MESSAGE_URL + url);
  }
}
