import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, map, Observable, of, switchMap, take, tap, throwError } from 'rxjs';
import { SigninRequestModel } from '../../models/auth/auth.signin.model';
import { TokenResponseModel } from '../../models/auth/auth.token.model';
import { SessionStorageService } from '../session-storage/session-storage.service';
import { SignupRequestModel } from '../../models/auth/auth.signup.model';
import { Response } from '../../models/response/response.model';
import { v4 as uuidv4 } from 'uuid';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl: string = environment.authUrl;
  private accessToken$ = new BehaviorSubject<string | null>(null);
  private authenticated$ = new BehaviorSubject<boolean>(false);

  isAuthenticated$: Observable<boolean> = this.authenticated$.asObservable();

  constructor(
    private http: HttpClient,
    private sessionService: SessionStorageService
  ) {
    if (this.getToken()) {
      this.authenticated$.next(true);
    }
  }

  private setToken(token: string) {
    this.accessToken$.next(token);
    this.sessionService.setAccessToken(token);
  }

  getToken(): string | null {
    return this.accessToken$.value ?? this.sessionService.getAccessToken();
  }

  private generateDeviceId() {
    var deviceId = uuidv4();
    this.sessionService.setDeviceId(deviceId);

    return deviceId;
  }

  private setAuthData(response: any) {
    this.setToken(response.accessToken);
    this.authenticated$.next(true);
  }

  signin(credentials: SigninRequestModel): Observable<TokenResponseModel> {
    const url = '/api/authentication/login';

    credentials.DeviceId = this.generateDeviceId();

    return this.http.post<TokenResponseModel>(this.baseUrl + url, credentials, { withCredentials: true })
      .pipe(
        map((response: any) => {
          this.setAuthData(response);

          return new TokenResponseModel(response.userId);
        }),
        catchError((error) => {
          // Handle the error here
          let response: Response = error.error;
          console.log(response);
          return throwError(() => response);
        })
      );
  }

  signup(credentials: SignupRequestModel): Observable<TokenResponseModel> {
    const url = '/api/authentication/register';

    credentials.DeviceId = this.generateDeviceId();

    return this.http.post<TokenResponseModel>(this.baseUrl + url, credentials)
      .pipe(
        map((response: any) => {
          this.setAuthData(response);

          return new TokenResponseModel(response.userId);
        }),
        catchError((error) => {
          // Handle the error here
          let response: Response = error.error;
          console.log(response);

          return throwError(() => response);
        })
      );
  }

  refresh(): Observable<TokenResponseModel> {
    const url = '/api/authentication/refresh';

    let body = {
      userId: this.sessionService.getUserInfo()!.Id,
      deviceId: this.sessionService.getDeviceId()
    };

    return this.http.post<TokenResponseModel>(this.baseUrl + url, body, { withCredentials: true })
      .pipe(
        tap((response: any) => {
          console.log('refresh success');
          this.setAuthData(response);
        }),
        catchError((error: HttpErrorResponse) => {
          console.log('refresh error');
          return throwError(() => error.error);
        })
      );
  }

  signout() {
    this.sessionService.logout();
    this.authenticated$.next(false);

    const url = '/api/authentication/logout';
    return this.http.delete<void>(this.baseUrl + url);
  }
}
