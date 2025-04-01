import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
  HttpClient,
} from '@angular/common/http';
import { catchError, Observable, switchMap, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { SessionStorageService } from '../services/session-storage/session-storage.service';
import { AuthService } from '../services/auth/auth.service';
import { environment } from '../../../environments/environment';
import { TokenResponseModel } from '../models/auth/auth.token.model';

@Injectable()
export class ResponseInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private sessionService: SessionStorageService,
    private authService: AuthService,
    private http: HttpClient
  ) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const excludedRoutes = ['/login', '/register'];

    const isExcluded = excludedRoutes.some(route => req.url.includes(route));

    if (isExcluded) {
      return next.handle(req); // Просто пропускаем запрос
    }

    return next.handle(req).pipe(
      catchError((error: any) => {
        if (error instanceof HttpErrorResponse) {
          if (error.status === 400) {
            this.router.navigate(['/bad-request']);
          } else if (error.status === 401) {
            return this.handle401Error(req, next);
          } else if (error.status === 403) {
            this.router.navigate(['/forbidden']);
          } else if (error.status === 404) {
            this.router.navigate(['/not-found']);
          }
        }

        console.log('bad intercept');
        return throwError(() => error);
      })
    );
  }

  private handle401Error(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.authService.refresh().pipe(
      switchMap((response: TokenResponseModel) => {
        const clonedRequest = req.clone({
          setHeaders: {
            Authorization: `Bearer ${response.AccessToken}`,
          }
        });

        return next.handle(clonedRequest);
      }),
      catchError((tokenError) => {
        this.router.navigate(['/unauthorized']);
        return throwError(() => tokenError);
      })
    );
  }
}
