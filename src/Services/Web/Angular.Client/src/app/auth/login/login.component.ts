import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthService } from '../../common/services/auth/auth.service';
import { SessionStorageService } from '../../common/services/session-storage/session-storage.service';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from '../../common/auth/auth.config';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  showErrors = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private sessionService: SessionStorageService,
    private oauthService: OAuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: [''],
      password: ['']
    });
  }

  ngOnInit() {
    this.oauthService.configure(authConfig);
    // this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }

  onSubmit() {
    const credentials = this.loginForm.value;
    this.authService.login(credentials).subscribe({
      next: (result) => {
        console.log(result);
        this.sessionService.setAccessToken(result.AccessToken);
        this.sessionService.setRefreshToken(result.RefreshToken);

        window.location.href = '/';
      },
      error: (error) => {
        this.showErrors = true;
      },
      complete: () => {
      }
    });
  }
}
