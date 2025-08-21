import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, map, Observable, of, switchMap, throwError } from 'rxjs';
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

  constructor(
    private http: HttpClient,
    private sessionService: SessionStorageService
  ) { }

  private setToken(token: string) {
    this.accessToken$.next(token);
    this.sessionService.setAccessToken(token);
  }

  getToken(): string | null { return this.accessToken$.value ?? this.sessionService.getAccessToken(); }

  isAuthenticated(): boolean {
    if (this.getToken()) {
      // const payload = JSON.parse(atob(token.split('.')[1]));
      // return payload.exp > Date.now() / 1000;

      return true;
    }

    return false;
  }

  private generateDeviceId() {
    var deviceId = uuidv4();
    this.sessionService.setDeviceId(deviceId);

    return deviceId;
  }

  signin(credentials: SigninRequestModel): Observable<TokenResponseModel> {
    const url = '/api/authentication/login';

    credentials.DeviceId = this.generateDeviceId();

    return this.http.post<TokenResponseModel>(this.baseUrl + url, credentials, { withCredentials: true })
      .pipe(
        map((response: any) => {
          this.setToken(response.accessToken);

          this.sessionService.setItem(this.sessionService.userIdKey, response.userId);

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
          this.setToken(response.accessToken);

          this.sessionService.setItem(this.sessionService.userIdKey, response.userId);

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
      userId: this.sessionService.getItem(this.sessionService.userIdKey),
      deviceId: this.sessionService.getDeviceId()
    };

    return this.http.post<TokenResponseModel>(this.baseUrl + url, body, { withCredentials: true })
      .pipe(
        switchMap((response: any) => {
          this.setToken(response.accessToken);

          this.sessionService.setItem(this.sessionService.userIdKey, response.userId);

          return of(new TokenResponseModel(response.userId));
        }),
        catchError((error) => {
          // Handle the error here
          let response: Response = error.error;
          console.log(response);
          return throwError(() => response);
        })
      );
  }

  signout() {
    const url = '/api/authentication/logout';
    return this.http.delete(this.baseUrl + url);
  }
}
