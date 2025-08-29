import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthEventService {
  private refreshToken$ = new Subject<void>();

  onRefreshToken(): Observable<void> {
    return this.refreshToken$.asObservable();
  }

  emitRefreshToken() {
    this.refreshToken$.next();
  }
}
