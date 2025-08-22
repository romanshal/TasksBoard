import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, filter, finalize, Observable, switchMap, take, throwError } from "rxjs";
import { AuthService } from "../services/auth/auth.service";
import { Router } from "@angular/router";

@Injectable()
export class AuthRequestInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(
    private authService: AuthService,
    private router: Router,
  ) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();
    let authReq = req;

    if (token) {
      authReq = this.addToken(req, token);
    }

    const isRefreshRequest = req.url.includes('/api/authentication/refresh');

    return next.handle(authReq).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse && !isRefreshRequest) {
          if (error.status === 400) {
            this.router.navigate(['/bad-request']);
          } else if (error.status === 401) {
            return this.handle401Error(authReq, next);
          } else if (error.status === 403) {
            this.router.navigate(['/forbidden']);
          } else if (error.status === 404) {
            this.router.navigate(['/not-found']);
          }
        }

        return throwError(() => error);
      })
    );
  }

  private addToken(req: HttpRequest<any>, token: string) {
    return req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        'X-Requested-With': 'XMLHttpRequest'
      }
    });
  }

  private handle401Error(req: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      console.log('handle');

      return this.authService.refresh().pipe(
        switchMap(() => {
          const token = this.authService.getToken();
          this.refreshTokenSubject.next(token);
          return next.handle(this.addToken(req, token!));
        }),
        catchError(err => {
          this.authService.signout();
          this.router.navigate(['/signin']);
          return throwError(() => err);
        }),
        finalize(() => {
          this.isRefreshing = false;
        })
      );
    } else {
      // ждем пока обновится токен
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(token => next.handle(this.addToken(req, token!)))
      );
    }
  }
}
