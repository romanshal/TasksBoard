import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../common/services/auth/auth.service';
import { OAuthService } from 'angular-oauth2-oidc';
import { ActivatedRoute } from '@angular/router';
import { Response } from '../../common/models/response/response.model';

@Component({
  selector: 'app-signup',
  standalone: false,
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent {
  signupForm!: FormGroup;

  showErrors = false;
  errorMessage?: string;

  formSubmitted = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private oauthService: OAuthService,
    private route: ActivatedRoute
  ) {
    this.signupForm = this.fb.group({
      username: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  get f() {
    return this.signupForm.controls;
  }

  onSubmit() {
    this.formSubmitted = true;

    if (this.signupForm.invalid) {
      return;
    }

    const credentials = this.signupForm.value;

    this.authService.signup(credentials).subscribe({
      next: (result) => {
        const returnUrl = this.route.snapshot.queryParams['returnurl'] || '/';

        window.location.href = returnUrl;
      },
      error: (error: Response) => {
        this.showErrors = true;
        this.errorMessage = error.Description;
      },
      complete: () => {
      }
    });
  }

}
