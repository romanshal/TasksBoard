import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ManageBoardMemberService {
  private MANAGE_URL: string = environment.apiUrl;

  constructor(
    private http: HttpClient,
  ) { }

  // addMember(boardId: string, member: any){
  //   const url = '/api/managemembers/board/' + boardId;
  //   return this.http.post(this.MANAGE_URL + url, member);
  // }

  updateMemberPermissions(boardId: string, permissions: any) {
    const url = '/api/managemembers/permissions/board/' + boardId ;
    return this.http.post(this.MANAGE_URL + url, permissions);
  }

  deleteMember(boardId: string, memberId: string) {
    const url = '/api/managemembers/board/' + boardId+ '/member/' + memberId;
    return this.http.delete(this.MANAGE_URL + url);
  }
}
