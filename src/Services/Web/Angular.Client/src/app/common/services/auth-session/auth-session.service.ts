import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { SessionStorageService } from '../session-storage/session-storage.service';
import { UserInfoModel } from '../../models/user/user-info.model';
import { AccessTokenPair } from '../../models/auth/auth.access-token-pair.model';
import { jwtDecode, JwtPayload } from 'jwt-decode';
import { AuthTokenTimerService } from '../auth-token-timer/auth-token-timer.service';

@Injectable({
  providedIn: 'root'
})
export class AuthSessionService {
  private accessTokenSubject$ = new BehaviorSubject<AccessTokenPair | null>(null);
  private currentUserSubject$ = new BehaviorSubject<UserInfoModel | null>(null);

  constructor(
    private sessionStorageService: SessionStorageService,
    private tokenTimerService: AuthTokenTimerService
  ) {
    const token = this.sessionStorageService.getAccessToken();
    if (token) {
      this.accessTokenSubject$.next(token);

      let user = this.sessionStorageService.getUserInfo();
      if (user) {
        this.currentUserSubject$.next(user);
      }
    }
  }

  isAuthenticated$: Observable<boolean> = this.accessTokenSubject$
    .pipe(
      map(token => !!token)
    );

  currentUser$: Observable<UserInfoModel | null> = this.currentUserSubject$.asObservable();

  getAccessToken(): string | null {
    return this.accessTokenSubject$.value?.Token ?? this.sessionStorageService.getAccessToken()?.Token ?? null;
  }

  getAccessTokenExpiry(): number | null {
    return this.accessTokenSubject$.value?.Expired ?? this.sessionStorageService.getAccessToken()?.Expired ?? null;
  }

  setAccessToken(token: string) {
    const exp = this.getExpiry(token);
    const tokenPair = new AccessTokenPair(token, exp!);

    this.accessTokenSubject$.next(tokenPair);
    this.sessionStorageService.setAccessToken(tokenPair);

    this.tokenTimerService.start(exp);
  }

  setCurrentUser(user: UserInfoModel) {
    this.currentUserSubject$.next(user);
    this.sessionStorageService.setUserInfo(user);
  }

  getCurrentUser() {
    return this.currentUserSubject$.value ?? this.sessionStorageService.getUserInfo();
  }

  setDeviceId(deviceId: string){
    this.sessionStorageService.setDeviceId(deviceId);
  }

  getDeviceId(): string | null{
    return this.sessionStorageService.getDeviceId();
  }

  logout() {
    this.sessionStorageService.logout();
    this.accessTokenSubject$.next(null);
    this.currentUserSubject$.next(null);

    this.tokenTimerService.stop();
  }

  private getExpiry(token: string): number {
    const payload = jwtDecode<JwtPayload>(token);
    if (!payload.exp) {
      throw new Error('Токен не содержит exp');
    }

    return payload.exp * 1000;
  }
}
