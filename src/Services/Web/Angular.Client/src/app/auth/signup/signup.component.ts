import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../common/services/auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Response } from '../../common/models/response/response.model';
import { UserService } from '../../common/services/user/user.service';
import { AuthSessionService } from '../../common/services/auth-session/auth-session.service';

@Component({
  selector: 'app-signup',
  standalone: false,
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent implements OnInit {
  signupForm!: FormGroup;

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
    this.signupForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(20)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  ngOnInit(){
    this.returnUrl = this.route.snapshot.queryParams['returnurl'] || '/';
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
        this.errorMessage = error.Description;
      }
    });
  }
}
