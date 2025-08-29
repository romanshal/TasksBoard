import { Component, OnInit } from '@angular/core';
import { AuthTokenTimerService } from './common/services/auth-token-timer/auth-token-timer.service';
import { AuthSessionService } from './common/services/auth-session/auth-session.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'ClickingUp | Everthing is possible';

  constructor(
    private tokenTimerService: AuthTokenTimerService,
    private authSessionService: AuthSessionService
  ) { }

  ngOnInit() {
    const exp = this.authSessionService.getAccessTokenExpiry();
    if (exp) {
      this.tokenTimerService.start(exp);
    }
  }
}
