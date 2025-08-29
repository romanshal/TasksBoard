import { Injectable, OnDestroy } from '@angular/core';
import { filter, Subscription, tap, timer } from 'rxjs';
import { AuthEventService } from '../auth-event/auth-event.service';

@Injectable({
  providedIn: 'root'
})
export class AuthTokenTimerService implements OnDestroy {
  private sub?: Subscription;
  private refreshBeforeSec = 60;

  constructor(
    private authEvent: AuthEventService,
  ) { }

start(exp: number) {
    this.stop();

    this.sub = timer(0, 10000)
    .pipe(
      filter(() => {
        return !!exp && exp - Date.now() <= this.refreshBeforeSec * 1000
      }),
      tap(() => this.authEvent.emitRefreshToken())
    ).subscribe();
  }

  stop() {
    this.sub?.unsubscribe();
  }

  ngOnDestroy() {
    this.stop();
  }
}
