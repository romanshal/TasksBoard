import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { SigninRequestModel } from '../../models/auth/auth.signin.model';
import { TokenResponseModel } from '../../models/auth/auth.token.model';
import { SessionStorageService } from '../session-storage/session-storage.service';
import { SignupRequestModel } from '../../models/auth/auth.signup.model';
import { Response } from '../../models/response/response.model';
import { AuthSessionService } from '../auth-session/auth-session.service';
import { AuthEventService } from '../auth-event/auth-event.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // private baseUrl: string = environment.authUrl;
  private baseUrl: string = environment.gatewayUrl;

  constructor(
    private http: HttpClient,
    private sessionStorageService: SessionStorageService,
    private authSessionService: AuthSessionService,
    private authEventService: AuthEventService,
    private router: Router
  ) {
    this.authEventService.onRefreshToken().subscribe(() => {
      this.refresh().subscribe();
    });
  }

  signin(credentials: SigninRequestModel): Observable<TokenResponseModel> {
    const url = '/login';

    return this.http.post<TokenResponseModel>(this.baseUrl + url, credentials, { withCredentials: true })
      .pipe(
        map((response: any) => {
          if (response.result.twoFactorEnabled) {
            this.router.navigate(['/two-factor']);
          }

          this.authSessionService.setAccessToken(response.result.accessToken);
          this.authSessionService.setDeviceId(response.result.deviceId);

          return new TokenResponseModel(response.result.userId);
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
    const url = '/external';

    var redirectUrl = `${window.location.origin}/external-callback?returnUrl=${returnUrl}`;
    const params = new HttpParams()
      .append('provider', provider)
      .append('redirecturl', redirectUrl);

    const fullUrl = `${this.baseUrl}${url}?${params.toString()}`;

    window.location.href = fullUrl;
  }

  externalSigninCallback(accessToken: string, deviceId: string) {
    this.authSessionService.setAccessToken(accessToken);
    this.authSessionService.setDeviceId(deviceId);
  }

  signup(credentials: SignupRequestModel): Observable<TokenResponseModel> {
    const url = '/register';

    return this.http.post<TokenResponseModel>(this.baseUrl + url, credentials)
      .pipe(
        map((response: any) => {
          this.authSessionService.setAccessToken(response.result.accessToken);
          this.authSessionService.setDeviceId(response.result.deviceId);

          return new TokenResponseModel(response.result.userId);
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
    const url = '/refresh';

    let body = {
      userId: this.sessionStorageService.getUserInfo()!.id,
      deviceId: this.sessionStorageService.getDeviceId()
    };

    return this.http.post<TokenResponseModel>(this.baseUrl + url, body, { withCredentials: true })
      .pipe(
        tap((response: any) => {
          this.authSessionService.setAccessToken(response.result.accessToken);
        }),
        catchError((error: HttpErrorResponse) => {
          console.log('refresh error');
          return throwError(() => error.error);
        })
      );
  }

  forgotPassowrd(form: any) {
    const url = '/forgot-password';

    return this.http.post(this.baseUrl + url, form)
      .pipe(
        tap((response: any) => {

        }),
        catchError((error: HttpErrorResponse) => {
          console.log('forgot-password error');
          return throwError(() => error.error);
        })
      );
  }

  signout() {
    const url = '/logout';

    return this.http.delete<void>(this.baseUrl + url)
      .pipe(
        tap(() => {
          this.authSessionService.logout();
        }),
        catchError((error: HttpErrorResponse) => {
          console.log('logout error');
          return throwError(() => error.error);
        })
      );
  }
}
