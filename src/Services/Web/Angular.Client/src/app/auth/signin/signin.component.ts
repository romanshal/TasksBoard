import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthService } from '../../common/services/auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from '../../common/auth/auth.config';
import { Response } from '../../common/models/response/response.model';
import { UserService } from '../../common/services/user/user.service';
import { SessionStorageService } from '../../common/services/session-storage/session-storage.service';

@Component({
  selector: 'app-signin',
  standalone: false,
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.scss'
})
export class SigninComponent implements OnInit {
  signinForm: FormGroup;

  showErrors = false;
  errorMessage?: string;

  formSubmitted = false;

  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private sessionService: SessionStorageService,
    private oauthService: OAuthService,
    private route: ActivatedRoute
  ) {
    this.signinForm = this.fb.group({
      username: [''],
      password: ['']
    });
  }

  ngOnInit() {
    this.oauthService.configure(authConfig);
    // this.oauthService.loadDiscoveryDocumentAndTrysignin();
  }

  onSubmit() {
    this.formSubmitted = true;
    this.isLoading = true;

    if (this.signinForm.invalid) {
      return;
    }

    const credentials = this.signinForm.value;

    this.authService.signin(credentials).subscribe({
      next: (result) => {
        this.userService.getUserInfo(result.UserId).subscribe(result => {
          this.sessionService.setUserInfo(result);

          const returnUrl = this.route.snapshot.queryParams['returnurl'] || '/';

          window.location.href = returnUrl;

          this.isLoading = false;
        }, error => {
          this.isLoading = false;

          console.error(error);
        });
      },
      error: (error: Response) => {
        this.showErrors = true;

        this.isLoading = false;

        this.errorMessage = error.Description;
      },
      complete: () => {

      }
    });
  }
}
