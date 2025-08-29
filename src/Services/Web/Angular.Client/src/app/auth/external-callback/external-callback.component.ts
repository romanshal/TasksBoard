import { Component, OnInit } from '@angular/core';
import { UserService } from '../../common/services/user/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../common/services/auth/auth.service';
import { AuthSessionService } from '../../common/services/auth-session/auth-session.service';

@Component({
  selector: 'app-external-callback',
  standalone: false,
  templateUrl: './external-callback.component.html',
  styleUrl: './external-callback.component.css'
})
export class ExternalCallbackComponent implements OnInit {
  isLoading = false;

  constructor(
    private authService: AuthService,
    private authSessionService: AuthSessionService,
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    const accessToken = this.route.snapshot.queryParamMap.get('accessToken');
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
    const userId = this.route.snapshot.queryParamMap.get('userId')!;

    if (accessToken) {
      this.authService.externalSigninCallback(accessToken);
      this.userService.getUserInfo(userId).subscribe({
        next: (user) => {
          this.authSessionService.setCurrentUser(user);

          this.isLoading = false;
          this.router.navigate([returnUrl]);
        },
        error: (error: Response) => {
          this.isLoading = false;
          console.error(error);
        }
      });
    } else {
      this.router.navigate(['/signin']);
    }
  }
}
