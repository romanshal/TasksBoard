import { HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { SessionStorageService } from "../services/session-storage/session-storage.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private sessionService: SessionStorageService
  ) { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = this.sessionService.getAccessToken();

    if (token) {
      const cloned = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        }
      });


      cloned.headers.set('X-Requested-With', 'XMLHttpRequest');

      return next.handle(cloned);
    }

    return next.handle(req);
  }
}
