import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { HttpOptionService } from '../http-option/http-options.service';
import { map, Observable } from 'rxjs';
import { BoardMessageModel } from '../../models/board-message/board-message.model';
import { ResultResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class BoardMessageService {
  private BOARD_MESSAGE_URL: string = environment.chatUrl;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService
  ) { }

  getMessages(boardId: string, pageIndex: number, pageSize: number): Observable<BoardMessageModel[]> {
    const url = '/api/boardmessages/' + boardId;
    return this.http.get<ResultResponse<BoardMessageModel[]>>(this.BOARD_MESSAGE_URL + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize) })
      .pipe(
        map((response: any) => {
          let list: BoardMessageModel[] = [];
          if (response.result) {
            list = response.result.map((item: any) => {
              let message = new BoardMessageModel(
                item.id,
                item.memberId,
                item.accountId,
                item.memberNickname,
                item.message,
                item.createdAt,
                item.modifiedAt,
                item.isDeleted
              );

              return message;
            })
          }

          return list;
        })
      );
  }

  sendMessage(boardId: string, form: any) {
    const url = '/api/boardmessages/' + boardId;
    return this.http.post(this.BOARD_MESSAGE_URL + url, form);
  }

  editMessage(boardId: string, form: any) {
    const url = '/api/boardmessages/' + boardId;
    return this.http.put(this.BOARD_MESSAGE_URL + url, form);
  }

  deleteMessage(boardId: string, messageId: string) {
    const url = '/api/boardmessages/' + boardId + '/' + messageId;
    return this.http.delete(this.BOARD_MESSAGE_URL + url);
  }
}
