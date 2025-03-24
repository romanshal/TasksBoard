import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class ResponseInterceptor implements HttpInterceptor {
  constructor(private router: Router) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const excludedRoutes = ['/login', '/register'];

    const isExcluded = excludedRoutes.some(route => req.url.includes(route));

    if (isExcluded) {
     return next.handle(req); // Просто пропускаем запрос
    }

    return next.handle(req).pipe(
      catchError((error: any) => {
        console.log('error in interceptor');
        console.log(error);
        if (error instanceof HttpErrorResponse) {
          if (error.status === 400) {
            this.router.navigate(['/bad-request']);
          } else if (error.status === 401) {
            this.router.navigate(['/unauthorized']);
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
}
