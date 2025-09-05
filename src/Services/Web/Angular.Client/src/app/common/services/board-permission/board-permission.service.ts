import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import {  Observable } from 'rxjs';
import { ResultResponse, unwrapResponse } from '../../models/response/response.model';
import { BoardPermission } from '../../models/board-permission/board-permission.model';

@Injectable({
  providedIn: 'root'
})
export class BoardPermissionService {
  private PERMISSION_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient,
  ) { }

  getPermissions(): Observable<BoardPermission[]> {
    const url = '/permissions';
    return this.http
      .get<ResultResponse<BoardPermission[]>>(this.PERMISSION_URL + url)
      .pipe(unwrapResponse<BoardPermission[]>()) as Observable<BoardPermission[]>;
  }
}
