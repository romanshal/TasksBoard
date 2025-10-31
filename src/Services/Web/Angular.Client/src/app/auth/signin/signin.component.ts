import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthService } from '../../common/services/auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Response } from '../../common/models/response/response.model';
import { UserService } from '../../common/services/user/user.service';
import { AuthSessionService } from '../../common/services/auth-session/auth-session.service';

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

  returnUrl!: string;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private authSessionService: AuthSessionService,
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.signinForm = this.fb.group({
      usernameOrEmail: [''],
      password: [''],
      rememberMe: [false]
    });
  }

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParams['returnurl'] || '/';
  }

  onSubmit() {
    this.formSubmitted = true;
    this.isLoading = true;

    if (this.signinForm.invalid) {
      this.isLoading = false;
      return;
    }

    const credentials = this.signinForm.value;

    this.authService.signin(credentials).subscribe({
      next: (result) => {
        this.userService.getUserInfo(result.UserId).subscribe({
          next: (user) => {
            this.authSessionService.setCurrentUser(user);

            this.isLoading = false;
            this.router.navigate([this.returnUrl]);
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
        this.errorMessage = error.description;
      }
    });
  }

  onForgotPassword() {
    this.router.navigate(['/forgot-password']);
  }

  onSignup(){
    this.router.navigate(['/signup']);
  }

  externalSignin(provider: string) {
    this.isLoading = true;
    this.authService.externalSignin(provider, this.returnUrl);
  }
}
