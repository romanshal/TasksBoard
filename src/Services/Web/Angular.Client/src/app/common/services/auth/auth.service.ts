import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of, switchMap, throwError } from 'rxjs';
import { LoginRequestModel } from '../../models/auth/auth.login.model';
import { TokenResponseModel } from '../../models/auth/auth.token.model';
import { SessionStorageService } from '../session-storage/session-storage.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl: string = environment.authUrl;

  constructor(
    private http: HttpClient,
    private sessionService: SessionStorageService
  ) { }

  isAuthenticated(): boolean {
    const token = this.sessionService.getAccessToken();

    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp > Date.now() / 1000;
    }

    return false;
  }

  login(credentials: LoginRequestModel): Observable<TokenResponseModel> {
    const url = '/api/authentication/login';
    return this.http.post<TokenResponseModel>(this.baseUrl + url, credentials)
      .pipe(
        map((response: any) => {
          this.sessionService.setAccessToken(response.accessToken);
          this.sessionService.setRefreshToken(response.refreshToken);
          this.sessionService.setItem(this.sessionService.userIdKey, response.userId);
  
          return new TokenResponseModel(response.accessToken, response.refreshToken, response.userId);
        }),
        catchError((error) => {
          // Handle the error here
          console.error("Login failed", error);
          return throwError(() => new Error("Login error, please try again later."));
        })
      );
  }

  /*   register() : Observable {
      
    } */

  refresh(): Observable<TokenResponseModel> {
    const url = '/api/authentication/token';

    let body = { 
      userId: this.sessionService.getItem(this.sessionService.userIdKey), 
      refreshToken: this.sessionService.getRefreshToken() 
    };

    return this.http.post<TokenResponseModel>(this.baseUrl + url, body)
      .pipe(
        switchMap((response: any) => {
          this.sessionService.setAccessToken(response.accessToken);
          this.sessionService.setRefreshToken(response.refreshToken);
          this.sessionService.setItem(this.sessionService.userIdKey, response.userId);

          return of(new TokenResponseModel(response.accessToken, response.refreshToken, response.userId));
        }),
        catchError((error) => {
          // Handle the error here
          console.error("Refresh token failed", error);
          return throwError(() => new Error("Refresh token error, please try again later."));
        })
      );
  }
}
