import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { HttpOptionService } from '../http-option/http-options.service';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { NotificationModel } from '../../models/notification/notification.model';
import * as signalR from '@microsoft/signalr';
import { PaginatedList } from '../../models/paginated-list/paginated-list.model';
import { AuthSessionService } from '../auth-session/auth-session.service';
import { Response, ResultResponse, unwrapResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private baseUrl: string = environment.gatewayUrl;
  private NOTIFICATION_HUB_URL = environment.gatewayUrl + '/notification';

  private hubConnection?: signalR.HubConnection;
  private notificationsSubject = new BehaviorSubject<NotificationModel[]>([]);
  public notification$ = this.notificationsSubject.asObservable();

  private isAuthenticated = false;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService,
    private authSessionService: AuthSessionService
  ) {
    this.authSessionService.isAuthenticated$.subscribe(status => {
      this.isAuthenticated = status;
    })
    if (this.isAuthenticated) {
      let userId = this.authSessionService.getCurrentUser()?.id;
      this.getNewByAccountId(userId!).subscribe(result => {
        if (result) {
          this.notificationsSubject.next(result);
        }
      });

      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(this.NOTIFICATION_HUB_URL + '?accountId=' + userId!, {
          accessTokenFactory: () => {
            return this.authSessionService.getAccessToken()!;
          }
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: ctx => Math.min(1000 * (ctx.previousRetryCount + 1), 10000)
        })
        .build();

      this.startConnection();
    }
  }

  async startConnection(): Promise<any> {
    this.hubConnection!.on('ReceiveNotification', (data) => {
      console.log('DATA');
      console.log(data);
      let notification = new NotificationModel(
        data.id,
        data.type,
        new Map(Object.keys(data.payload).map(x => [x, data.payload[x]])),
        data.read,
        data.createdAt
      )

      console.log('NOTIFICATION');
      console.log(notification);

      const currentNotifications = this.notificationsSubject.value;
      this.notificationsSubject.next([notification, ...currentNotifications]);
    });

    try {
      await this.hubConnection!.start();
      return console.log('SignalR подключен');
    } catch (err) {
      return console.error(err);
    }
  }

  getAllPaginatedByAccountId(accountId: string, pageIndex: number, pageSize: number): Observable<PaginatedList<NotificationModel>> {
    const url = '/notifications/all/p/' + accountId;
    return this.http
      .get<ResultResponse<PaginatedList<NotificationModel>>>(this.baseUrl + url, {
        params: this.httpOption.getPaginationOptions(pageIndex, pageSize)
      })
      .pipe(unwrapResponse<PaginatedList<NotificationModel>>()) as Observable<PaginatedList<NotificationModel>>;
  }

  getNewPaginatedByAccountId(accountId: string, pageIndex: number, pageSize: number): Observable<PaginatedList<NotificationModel>> {
    const url = '/notifications/new/p/' + accountId;
    return this.http
      .get<ResultResponse<PaginatedList<NotificationModel>>>(this.baseUrl + url, {
        params: this.httpOption.getPaginationOptions(pageIndex, pageSize)
      })
      .pipe(unwrapResponse<PaginatedList<NotificationModel>>()) as Observable<PaginatedList<NotificationModel>>;
  }

  setRead(accountId: string, ids: string[]) {
    const url = '/notifications/' + accountId;
    return this.http
      .post<Response>(this.baseUrl + url, ids)
      .pipe(unwrapResponse<void>())
      .subscribe({
        next: () => {
          let currentNotifications = this.notificationsSubject.value;
          currentNotifications = currentNotifications.filter(notify => !ids.includes(notify.id))

          this.notificationsSubject.next(currentNotifications);
        }
      });
  }

  private getNewByAccountId(accountId: string): Observable<NotificationModel[]> {
    const url = '/notifications/new/' + accountId;
    return this.http
      .get<ResultResponse<NotificationModel[]>>(this.baseUrl + url)
      .pipe(unwrapResponse<NotificationModel[]>()) as Observable<NotificationModel[]>;
  }
}
