import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { UserInfoModel } from '../../models/user/user-info.model';
import { map, Observable, tap } from 'rxjs';
import { Response, ResultResponse, unwrapResponse } from '../../models/response/response.model';
import { UserImageModel } from '../../models/user/user-image.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl: string = environment.gatewayUrl;

  private static readonly MIME_TYPE_MAP: Record<string, string> = {
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.jpeg': 'image/jpeg'
  };

  constructor(
    private http: HttpClient
  ) { }

  getUserInfo(userId: string): Observable<UserInfoModel> {
    const url = '/user/' + userId;
    return this.http
      .get<ResultResponse<UserInfoModel>>(this.baseUrl + url)
      .pipe(unwrapResponse<UserInfoModel>()) as Observable<UserInfoModel>;
  }

  getUserAvatar(userId: string): Observable<string> {
    const url = '/image/' + userId;
    return this.http
      .get<ResultResponse<UserImageModel>>(this.baseUrl + url)
      .pipe(
        unwrapResponse<UserImageModel>(),
        map(response => {
          if (!response?.image) {
            return '';
          }

          const mimeType = UserService.MIME_TYPE_MAP[response.imageExtension ?? ''] || 'application/octet-stream';

          return response.image.startsWith('data:') ? response.image : `data:${mimeType};base64,${response.image}`;
        })
      );
  }

  updateUserInfo(userId: string, form: any): Observable<UserInfoModel> {
    const url = '/user/' + userId;
    return this.http
      .put<ResultResponse<UserInfoModel>>(this.baseUrl + url, form)
      .pipe(unwrapResponse<UserInfoModel>()) as Observable<UserInfoModel>;
  }

  changeUserPassword(userId: string, form: any): Observable<void> {
    const url = '/password/' + userId;
    return this.http
      .put<Response>(this.baseUrl + url, form)
      .pipe(unwrapResponse<void>());
  }

  updateUserAvatar(userId: string, image: any): Observable<void> {
    const url = '/image/' + userId;
    return this.http
      .put<Response>(this.baseUrl + url, image)
      .pipe(unwrapResponse<void>());
  };
}
