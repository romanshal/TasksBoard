import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../common/services/auth/auth.service';
import { OAuthService } from 'angular-oauth2-oidc';
import { ActivatedRoute, Router } from '@angular/router';
import { Response } from '../../common/models/response/response.model';
import { UserService } from '../../common/services/user/user.service';
import { SessionStorageService } from '../../common/services/session-storage/session-storage.service';

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

  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.signupForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(20)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  get f() {
    return this.signupForm.controls;
  }

  onSubmit() {
    this.formSubmitted = true;
    this.isLoading = true;

    if (this.signupForm.invalid) {
      return;
    }

    const credentials = this.signupForm.value;

    this.authService.signup(credentials).subscribe({
      next: (result) => {
        this.userService.getUserInfo(result.UserId).subscribe({
          next: () => {
            const returnUrl = this.route.snapshot.queryParams['returnurl'] || '/';
            this.isLoading = false;
            this.router.navigate([returnUrl]);
          },
          error: (error: Response) => {
            this.isLoading = false;
            console.error(error);
          }
        });
      },
      error: (error: Response) => {
        this.showErrors = true;
        this.isLoading = false;
        this.errorMessage = error.Description;
      }
    });
  }
}
