import { Component, OnInit } from '@angular/core';
import { AuthService } from '../common/services/auth/auth.service';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { Router } from '@angular/router';
import { UserService } from '../common/services/user/user.service';
import { MatDialog } from '@angular/material/dialog';
import { ProfileMenuModal } from '../common/modals/profile-menu/profile-menu.modal';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  public isAuthenticated = false;
  private userId?: string;
  public username!: string;

  constructor(
    private authService: AuthService,
    private sessionService: SessionStorageService,
    private router: Router,
    private userService: UserService,
    private dialog: MatDialog,
  ) {
    this.isAuthenticated = this.authService.isAuthenticated();
    if (this.isAuthenticated) {
      this.userId = this.sessionService.getItem(this.sessionService.userIdKey)!;
      this.userService.getUserInfo(this.userId).subscribe(result => {
        this.username = result.Username;
      });
    }
  }

  ngOnInit() {

  }

  signin() {
    this.router.navigate(['/login']);
  }

  signup() {
    this.router.navigate(['/register']);
  }

  openProfile() {
    this.dialog.open(ProfileMenuModal, {
      data: {
        username: this.username
      }
    })
  }
}
