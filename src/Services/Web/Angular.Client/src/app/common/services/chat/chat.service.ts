import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SessionStorageService } from '../session-storage/session-storage.service';
import { BoardMessageModel } from '../../models/board-message/board-message.model';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private CHAT_HUB_URL: string = environment.chatUrl + '/chat';

  private hubConnection: signalR.HubConnection;
  private messageSubject = new BehaviorSubject<any>(null);
  public messages$ = this.messageSubject.asObservable();

  constructor(
    private sessionService: SessionStorageService
  ) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.CHAT_HUB_URL, {
        accessTokenFactory: () => {
          return this.sessionService.getAccessToken()!;
        }
      })
      .build();
  }

  startConnection(): Promise<any> {
    this.hubConnection.on('ReceiveMessage', (data) => {
      let message = new BoardMessageModel (
        data.id,
        data.memberId,
        data.accountId,
        data.memberNickname,
        data.message,
        data.createdAt,
        data.modifiedAt
      )
      this.messageSubject.next(message);
    });

    return this.hubConnection.start()
      .then(() => console.log('SignalR подключен'))
      .catch(err => console.error(err));
  }

  joinBoard(boardId: string, userId: string) {
    this.startConnection().then(() => {
      return this.hubConnection.invoke('JoinBoard', boardId, userId);
    });
  }

  leaveBoard(boardId: string) {
    console.log('Leave from ' + boardId);
    return this.hubConnection.invoke('LeaveBoard', boardId);
  }
}
