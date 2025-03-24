import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthService } from '../../common/services/auth/auth.service';
import { SessionStorageService } from '../../common/services/session-storage/session-storage.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private sessionService: SessionStorageService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: [''],
      password: ['']
    });
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
        alert('Error!');
      },
      complete: () => {
      }
    });
  }
}
