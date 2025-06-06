import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { UserInfoModel } from '../../models/user/user-info.model';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl: string = environment.authUrl;

  constructor(
    private http: HttpClient
  ) { }

  getUserInfo(userId: string): Observable<UserInfoModel> {
    const url = '/api/manage/' + userId;
    return this.http.get<UserInfoModel>(this.baseUrl + url)
      .pipe(
        map((response: any) => {
          return new UserInfoModel
            (
              response.id,
              response.username,
              response.email,
              response.firstname,
              response.surname
            );
        })
      );
  }

  getUserAvatar(userId: string): Observable<string> {
    const url = '/api/manage/image/' + userId;
    return this.http.get(this.baseUrl + url)
      .pipe(
        map((response: any) => {
          let base64Image = '';
          if (response !== null && response?.image !== null) {
            const mimeTypeMap: Record<string, string> = {
              '.png': 'image/png',
              '.jpg': 'image/jpeg',
              '.jpeg': 'image/jpeg'
            };

            const mimeType = mimeTypeMap[response.imageExtension!] || 'application/octet-stream';
            base64Image = response.image!;

            if (!base64Image.startsWith('data:')) {
              base64Image = `data:${mimeType};base64,${base64Image}`;
            }
          }

          return base64Image;
        })
      );
  }

  updateUserInfo(userId: string, form: any) {
    const url = '/api/manage/info/' + userId;
    return this.http.post<string>(this.baseUrl + url, form);
  }

  changeUserPassword(userId: string, form: any) {
    const url = '/api/manage/password/' + userId;
    return this.http.post<string>(this.baseUrl + url, form);
  }

  updateUserAvatar(userId: string, image: any) {
    const url = '/api/manage/image/' + userId;
    return this.http.post(this.baseUrl + url, image);
  };
}
