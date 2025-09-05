import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Response, ResultResponse, unwrapResponse } from '../../models/response/response.model';

@Injectable({
  providedIn: 'root'
})
export class ManageBoardMemberService {
  private MANAGE_URL: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient,
  ) { }

  // addMember(boardId: string, member: any){
  //   const url = '/api/managemembers/board/' + boardId;
  //   return this.http.post(this.MANAGE_URL + url, member);
  // }

  updateMemberPermissions(boardId: string, permissions: any): Observable<string> {
    const url = '/members/permissions/board/' + boardId;
    return this.http
      .post<ResultResponse<string>>(this.MANAGE_URL + url, permissions)
      .pipe(unwrapResponse<string>()) as Observable<string>;
  }

  deleteMember(boardId: string, memberId: string): Observable<void> {
    const url = '/members/board/' + boardId + '/member/' + memberId;
    return this.http
      .delete<Response>(this.MANAGE_URL + url)
      .pipe(unwrapResponse<void>());
  }
}
