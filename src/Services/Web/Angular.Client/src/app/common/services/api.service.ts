import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl: string = environment.apiUrl;
  private boardController = '/api/boards'

  private boardByIdUrl = '/board/'

  constructor(
    private http: HttpClient
  ) { }

  getBoardById(boardId: string): Observable<any> {
    return this.http.get(this.baseUrl + this.boardController + this.boardByIdUrl + boardId);
  }
}
