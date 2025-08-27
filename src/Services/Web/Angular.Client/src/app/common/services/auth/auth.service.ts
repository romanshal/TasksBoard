import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { SigninRequestModel } from '../../models/auth/auth.signin.model';
import { TokenResponseModel } from '../../models/auth/auth.token.model';
import { SessionStorageService } from '../session-storage/session-storage.service';
import { SignupRequestModel } from '../../models/auth/auth.signup.model';
import { Response } from '../../models/response/response.model';
import { v4 as uuidv4 } from 'uuid';
import { AuthStateService } from '../auth-state/auth-state.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl: string = environment.authUrl;

  constructor(
    private http: HttpClient,
    private sessionService: SessionStorageService,
    private authStateService: AuthStateService
  ) { }

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
          this.authStateService.setAccessToken(response.accessToken);

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

  externalSignin(provider: string, returnUrl: string) {
    const url = '/api/authenticationexternal/login';
    const deviceId = this.generateDeviceId();

    var redirectUrl = `${window.location.origin}/external-callback?returnUrl=${returnUrl}`;
    const params = new HttpParams()
      .append('provider', provider)
      .append('deviceid', deviceId)
      .append('redirecturl', redirectUrl);

    const fullUrl = `${this.baseUrl}${url}?${params.toString()}`;

    window.location.href = fullUrl;
  }

  externalSigninCallback(accessToken: string) {
    this.authStateService.setAccessToken(accessToken);
  }

  signup(credentials: SignupRequestModel): Observable<TokenResponseModel> {
    const url = '/api/authentication/register';

    credentials.DeviceId = this.generateDeviceId();

    return this.http.post<TokenResponseModel>(this.baseUrl + url, credentials)
      .pipe(
        map((response: any) => {
          this.authStateService.setAccessToken(response.accessToken);

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
          this.authStateService.setAccessToken(response.accessToken);
        }),
        catchError((error: HttpErrorResponse) => {
          console.log('refresh error');
          return throwError(() => error.error);
        })
      );
  }

  signout() {
    const url = '/api/authentication/logout';

    return this.http.delete<void>(this.baseUrl + url)
      .pipe(
        tap(() => {
          this.authStateService.logout();
        }),
        catchError((error: HttpErrorResponse) => {
          console.log('logout error');
          return throwError(() => error.error);
        })
      );
  }
}
