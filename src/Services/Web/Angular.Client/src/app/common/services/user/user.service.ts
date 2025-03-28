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
          return new UserInfoModel(response.username, response.email)
        })
      );
  }
}
