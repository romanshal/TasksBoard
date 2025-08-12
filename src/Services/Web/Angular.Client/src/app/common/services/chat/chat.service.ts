import { Injectable, NgZone } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SessionStorageService } from '../session-storage/session-storage.service';
import { BoardMessageModel } from '../../models/board-message/board-message.model';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private CHAT_HUB_URL: string = environment.chatUrl + '/chat';

  private connection?: signalR.HubConnection;
  // private messageSubject = new BehaviorSubject<BoardMessageModel[]>([]);
  // public messages$ = this.messageSubject.asObservable();
  private newMessagesSub = new Subject<BoardMessageModel>();
  private editMessageSub = new Subject<BoardMessageModel>();
  private deleteMessageSub = new Subject<string>();
  private boardId: string | null = null;
  private userId: string | null = null;
  private stateSub = new BehaviorSubject<'disconnected' | 'connecting' | 'connected'>('disconnected');

  constructor(
    private sessionService: SessionStorageService,
    private ngZone: NgZone,
  ) {
  }

  onMessage(): Observable<BoardMessageModel> {
    return this.newMessagesSub.asObservable();
  }

  onEdit(): Observable<BoardMessageModel> {
    return this.editMessageSub.asObservable();
  }

  onDelete(): Observable<string> {
    return this.deleteMessageSub.asObservable();
  }

  connectionState(): Observable<'disconnected' | 'connecting' | 'connected'> {
    return this.stateSub.asObservable();
  }

  async startConnection(): Promise<void> {
    if (this.connection && this.connection.state !== signalR.HubConnectionState.Disconnected) return;

    this.stateSub.next('connecting');

    await this.ngZone.runOutsideAngular(async () => {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(this.CHAT_HUB_URL, {
          accessTokenFactory: () => {
            return this.sessionService.getAccessToken()!;
          }
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: ctx => Math.min(1000 * (ctx.previousRetryCount + 1), 10000)
        })
        .build();

      this.connection.on('ReceiveMessage', (data) => {
        let message = new BoardMessageModel(
          data.value.id,
          data.value.memberId,
          data.value.accountId,
          data.value.memberNickname,
          data.value.message,
          data.value.createdAt,
          data.value.modifiedAt,
          data.value.IsDeleted
        )

        this.ngZone.run(() => this.newMessagesSub.next(message));
      });

      this.connection.on('EditMessage', (data) => {
        let message = new BoardMessageModel(
          data.value.id,
          data.value.memberId,
          data.value.accountId,
          data.value.memberNickname,
          data.value.message,
          data.value.createdAt,
          data.value.modifiedAt,
          data.value.IsDeleted
        );

        this.ngZone.run(() => this.editMessageSub.next(message));
      });

      this.connection.on('DeleteMessage', (data) => {
        console.log('delete message');
        console.log(data);

        this.ngZone.run(() => this.deleteMessageSub.next(data));
      });

      this.connection.onreconnected(async () => {
        if (this.boardId && this.userId) {
          try {
            await this.startConnection();
            await this.switchBoard(this.boardId, this.userId);
          } catch (e) {

          }
        }
      });

      this.connection.onclose(() => {
        this.ngZone.run(() => this.stateSub.next('disconnected'));
      });

      await this.connection.start();
    });

    this.stateSub.next('connected');

    if (this.boardId && this.userId) {
      console.log('Exect join board on front');
      await this.safeInvoke('JoinBoard', this.boardId, this.userId);
    }
  }

  async switchBoard(boardId: string, userId: string): Promise<void> {
    this.boardId = boardId;
    this.userId = userId;

    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      await this.startConnection();
    }

    await this.safeInvoke('JoinBoard', boardId, userId);
  }

  async stop(): Promise<void> {
    if (this.connection) {
      try {
        if (this.boardId) {
          await this.safeInvoke('LeaveBoard', this.boardId);
        }

        await this.connection.stop();
      } finally {
        this.stateSub.next('disconnected');
      }
    }
  }

  private async safeInvoke(method: string, ...args: any[]): Promise<void> {
    if (!this.connection) throw new Error('SignalR connection is not initialized');
    try {
      await this.connection.invoke(method, ...args);
    } catch (err) {
      console.log('Error when invoke: ' + method);
      console.log((err as Error).message);
      console.log((err as Error).stack);
      throw err;
    }
  }
}
