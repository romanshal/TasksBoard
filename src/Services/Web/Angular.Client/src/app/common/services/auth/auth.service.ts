import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { LoginRequestModel } from '../../models/auth/auth.login.model';
import { TokenResponseModel } from '../../models/auth/auth.token.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl: string = environment.authUrl;

  constructor(
    private http: HttpClient
  ) { }

  isAuthenticated(): boolean {
    const token = sessionStorage.getItem('access_token');

    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp > Date.now() / 1000;
    }

    return false;
  }

  login(credentials: LoginRequestModel): Observable<TokenResponseModel> {
    const url = "/api/authentication/login";
    return this.http.post<TokenResponseModel>(this.baseUrl + url, credentials)
    .pipe(
      map((response: any) => {
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
}
