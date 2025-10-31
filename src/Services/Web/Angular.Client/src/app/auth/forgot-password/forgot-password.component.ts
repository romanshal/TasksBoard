import { Component, Input, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../common/services/auth/auth.service';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Subscription } from 'rxjs/internal/Subscription';
import { interval } from 'rxjs/internal/observable/interval';
import { map } from 'rxjs/internal/operators/map';
import { takeWhile } from 'rxjs/internal/operators/takeWhile';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  standalone: false,
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent implements OnDestroy {
  fpForm: FormGroup;
  formSubmitted = false;
  isLoading = false;
  success = false;

  private readonly START_SECONDS = 20;
  isRunning = false;
  secondsLeft = 0;
  private sub: Subscription | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.fpForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  start(): void {
    this.stopTimer();
    this.secondsLeft = this.START_SECONDS;
    this.isRunning = true;

    this.sub = interval(1000).subscribe(() => {
      this.secondsLeft = Math.max(0, this.secondsLeft - 1);
      if (this.secondsLeft === 0) {
        this.stopTimer();
      }
    });
  }

  private stopTimer(): void {
    if (this.sub) {
      this.sub.unsubscribe();
      this.sub = null;
    }
    this.isRunning = false;
  }

  onSubmit() {
    this.formSubmitted = true;
    this.isLoading = true;

    if (this.fpForm.invalid) {
      this.isLoading = false;
      return;
    }

    this.authService.forgotPassowrd(this.fpForm.value).subscribe({
      next: () => {
        this.success = true;
        this.isLoading = false;
        this.start();
      },
      error: (error: Response) => {
        this.isLoading = false;
        console.error(error);
      }
    });
  }

  onBack() {
    this.router.navigate(['/signin']);
  }

  ngOnDestroy(): void {
    this.stopTimer();
  }
}
