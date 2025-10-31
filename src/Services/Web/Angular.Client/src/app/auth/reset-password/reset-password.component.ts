import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AuthService } from '../../common/services/auth/auth.service';
import { Router } from '@angular/router';

function passwordsMatchValidator(passwordKey = 'password', confirmKey = 'confirmPassword'): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const pw = group.get(passwordKey)?.value;
    const cpw = group.get(confirmKey)?.value;
    if (pw == null || cpw == null) return null;
    return pw === cpw ? null : { passwordsMismatch: true };
  };
}

@Component({
  selector: 'app-reset-password',
  standalone: false,
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss'
})
export class ResetPasswordComponent {
  rpForm: FormGroup;
  formSubmitted = false;
  isLoading = false;
  success = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.rpForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordsMatchValidator() });
  }

  onSubmit() {
    this.formSubmitted = true;
    this.isLoading = true;

    if (this.rpForm.invalid) {
      this.isLoading = false;
      return;
    }

    this.success = true;
  }

  onBack(){
    this.router.navigate(['/signin']);
  }
}
