import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { SessionStorageService } from '../session-storage/session-storage.service';
import { UserInfoModel } from '../../models/user/user-info.model';

@Injectable({
  providedIn: 'root'
})
export class AuthStateService {
  private accessTokenSubject$ = new BehaviorSubject<string | null>(null);
  private authenticatedSubject$ = new BehaviorSubject<boolean>(false);
  isAuthenticated$: Observable<boolean> = this.authenticatedSubject$.asObservable();

  private currentUserSubject$ = new BehaviorSubject<UserInfoModel | null>(null);
  currentUser$: Observable<UserInfoModel | null> = this.currentUserSubject$.asObservable();

  constructor(
    private sessionStorageService: SessionStorageService
  ) {
    if (this.getAccessToken()) {
      this.authenticatedSubject$.next(true);

      let user = this.sessionStorageService.getUserInfo();
      if (user) {
        this.currentUserSubject$.next(user);
      }
    }
  }

  getAccessToken(): string | null {
    return this.accessTokenSubject$.value ?? this.sessionStorageService.getAccessToken();
  }

  setAccessToken(token: string) {
    this.accessTokenSubject$.next(token);
    this.sessionStorageService.setAccessToken(token);
    this.authenticatedSubject$.next(true);
  }

  setCurrentUser(user: UserInfoModel) {
    this.currentUserSubject$.next(user);
    this.sessionStorageService.setUserInfo(user);
  }

  getCurrentUser() {
    return this.currentUserSubject$.value ?? this.sessionStorageService.getUserInfo();
  }

  logout() {
    this.sessionStorageService.logout();
    this.authenticatedSubject$.next(false);
  }
}
