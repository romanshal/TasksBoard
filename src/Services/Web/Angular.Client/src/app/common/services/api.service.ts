import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl: string = environment.apiUrl;
  private getBoardsUrl = '/api/boards/'

  constructor(
    private http: HttpClient
  ) { }

  get<T>(): Observable<T> {
    return this.http.get<T>(this.baseUrl + this.getBoardsUrl);
  }
}
