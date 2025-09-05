import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { UserInfoModel } from '../../models/user/user-info.model';
import { ResultResponse } from '../../models/response/response.model';
import { HttpOptionService } from '../http-option/http-options.service';
import { SearchModel } from '../../models/user/search-info.model';

@Injectable({
  providedIn: 'root'
})
export class SearchMemberService {
  private SEARCH_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient,
    private httpOptionService: HttpOptionService
  ) { }

  seacrhMember(query: string): Observable<SearchModel[]> {
    const url = '/earch';
    return this.http.get<ResultResponse<SearchModel>>(this.SEARCH_URL + url, { params: this.httpOptionService.getOptions('search', query) })
      .pipe(
        map((response: any) => {
          let list: SearchModel[] = [];
          if (response.result) {
            list = response.result.map((item: any) => {
              let user = new SearchModel(
                item.userId,
                item.username,
              );

              return user;
            });
          }

          return list;
        })
      );
  }
}
