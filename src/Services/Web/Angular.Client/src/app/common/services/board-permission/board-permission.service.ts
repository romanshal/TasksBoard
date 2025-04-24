import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { BoardMemberPermission } from '../../models/board-member-permission/board-member-permission.model';
import { map, Observable } from 'rxjs';
import { ResultResponse } from '../../models/response/response.model';
import { BoardPermission } from '../../models/board-permission/board-permission.model';

@Injectable({
  providedIn: 'root'
})
export class BoardPermissionService {
  private PERMISSION_URL: string = environment.apiUrl;

  constructor(
    private http: HttpClient,
  ) { }

  getPermissions(): Observable<BoardPermission[]> {
    const url = '/api/permissions';
    return this.http.get<ResultResponse<BoardPermission>>(this.PERMISSION_URL + url)
      .pipe(
        map((response: any) => {
          let list: BoardPermission[] = [];
          if (response.result) {
            list = response.result.map((item: any) => {
              let permission = new BoardPermission();
              permission.Id = item.id;
              permission.Name = item.name;
              permission.AccessLevel = item.accessLevel;

              return permission;
            });
          }

          return list;
        })
      );
  }
}
