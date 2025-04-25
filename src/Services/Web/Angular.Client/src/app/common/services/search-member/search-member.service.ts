import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { UserInfoModel } from '../../models/user/user-info.model';
import { ResultResponse } from '../../models/response/response.model';
import { HttpOptionService } from '../http-option/http-options.service';

@Injectable({
  providedIn: 'root'
})
export class SearchMemberService {
  private SEARCH_URL: string = environment.authUrl;

  constructor(
    private http: HttpClient,
    private httpOptionService: HttpOptionService
  ) { }

  seacrhMember(query: string): Observable<UserInfoModel[]> {
    const url = '/api/search';
    return this.http.get<ResultResponse<UserInfoModel>>(this.SEARCH_URL + url, { params: this.httpOptionService.getOptions('search', query) })
      .pipe(
        map((response: any) => {
          let list: UserInfoModel[] = [];
          if (response.result) {
            list = response.result.map((item: any) => {
              let user = new UserInfoModel(
                item.username,
                item.email
              );

              return user;
            });
          }

          return list;
        })
      );
  }
}
