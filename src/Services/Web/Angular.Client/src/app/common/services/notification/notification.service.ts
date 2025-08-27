import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { HttpOptionService } from '../http-option/http-options.service';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { NotificationModel } from '../../models/notification/notification.model';
import * as signalR from '@microsoft/signalr';
import { PaginatedList } from '../../models/paginated-list/paginated-list.model';
import { UserService } from '../user/user.service';
import { AuthStateService } from '../auth-state/auth-state.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private baseUrl: string = environment.notificationUrl;
  private NOTIFICATION_HUB_URL = environment.notificationUrl + '/notification';

  private hubConnection?: signalR.HubConnection;
  private notificationsSubject = new BehaviorSubject<NotificationModel[]>([]);
  public notification$ = this.notificationsSubject.asObservable();

  private isAuthenticated = false;

  constructor(
    private http: HttpClient,
    private httpOption: HttpOptionService,
    private userService: UserService,
    private authStateService: AuthStateService
  ) {
    this.authStateService.isAuthenticated$.subscribe(status => {
      this.isAuthenticated = status;
    })
    if (this.isAuthenticated) {
      let userId = this.authStateService.getCurrentUser()?.Id;
      this.getNewByAccountId(userId!).subscribe(result => {
        if (result) {
          this.notificationsSubject.next(result);
        }
      });

      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(this.NOTIFICATION_HUB_URL + '?accountId=' + userId!, {
          accessTokenFactory: () => {
            return this.authStateService.getAccessToken()!;
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
      let notification = new NotificationModel(
        data.id,
        data.type,
        new Map(Object.keys(data.payload).map(x => [x, data.payload[x]])),
        data.read,
        data.createdAt
      )

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
    const url = '/api/notifications/all/paginated/' + accountId;
    return this.http.get(this.baseUrl + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize) })
      .pipe(
        map((response: any) => {
          const paginatedList = new PaginatedList<NotificationModel>();
          if (response.result?.items) {
            paginatedList.Items = response.result.items.map((item: any) => {
              let notification = new NotificationModel(
                item.id,
                item.type,
                new Map(Object.keys(item.payload).map(x => [x, item.payload[x]])),
                item.read,
                item.createdAt
              );

              return notification;
            });
          }

          paginatedList.TotalCount = response.result.totalCount;
          paginatedList.PageIndex = response.result.pageIndex;
          paginatedList.PageSize = response.result.pageSize;
          paginatedList.PagesCount = response.result.pagesCount

          return paginatedList;
        }));
  }

  getNewPaginatedByAccountId(accountId: string, pageIndex: number, pageSize: number): Observable<PaginatedList<NotificationModel>> {
    const url = '/api/notifications/new/paginated/' + accountId;
    return this.http.get(this.baseUrl + url, { params: this.httpOption.getPaginationOptions(pageIndex, pageSize) })
      .pipe(
        map((response: any) => {
          const paginatedList = new PaginatedList<NotificationModel>();
          if (response.result?.items) {
            paginatedList.Items = response.result.items.map((item: any) => {
              let notification = new NotificationModel(
                item.id,
                item.type,
                new Map(Object.keys(item.payload).map(x => [x, item.payload[x]])),
                item.read,
                item.createdAt
              );

              return notification;
            });
          }

          paginatedList.TotalCount = response.result.totalCount;
          paginatedList.PageIndex = response.result.pageIndex;
          paginatedList.PageSize = response.result.pageSize;
          paginatedList.PagesCount = response.result.pagesCount

          return paginatedList;
        }));
  }

  setRead(accountId: string, ids: string[]) {
    const url = '/api/notifications/' + accountId;
    return this.http.post(this.baseUrl + url, ids).subscribe(result => {
      if (result) {
        let currentNotifications = this.notificationsSubject.value;
        currentNotifications = currentNotifications.filter(notify => !ids.includes(notify.Id))

        this.notificationsSubject.next(currentNotifications);
      }
    });
  }

  private getNewByAccountId(accountId: string): Observable<NotificationModel[]> {
    const url = '/api/notifications/new/' + accountId;
    return this.http.get(this.baseUrl + url)
      .pipe(
        map((response: any) => {
          let list: NotificationModel[] = [];
          if (response.result) {
            list = response.result.map((item: any) => {
              let notification = new NotificationModel(
                item.id,
                item.type,
                new Map(Object.keys(item.payload).map(x => [x, item.payload[x]])),
                item.read,
                item.createdAt
              );

              return notification;
            })
          }

          return list;
        })
      );
  }
}
