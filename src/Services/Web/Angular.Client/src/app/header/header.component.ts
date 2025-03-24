import { Component, OnInit } from '@angular/core';
import { AuthService } from '../common/services/auth/auth.service';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  public isAuthenticated = false;

  constructor(
    private authService: AuthService,
    private sessionService: SessionStorageService,
    private router: Router
  ) {

  }

  ngOnInit() {
    this.isAuthenticated = this.authService.isAuthenticated();
  }

  signin() {
    this.router.navigate(['/login']);
  }

  signup() {
    this.router.navigate(['/register']);
  }

  logout() {
    this.sessionService.logout();

    window.location.href = '/login';
  }
}
