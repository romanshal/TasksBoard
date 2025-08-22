import { AfterViewInit, Component, OnInit } from '@angular/core';
import { AuthService } from '../common/services/auth/auth.service';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { Router } from '@angular/router';
import { UserService } from '../common/services/user/user.service';
import { MatDialog } from '@angular/material/dialog';
import { ProfileMenuModal } from '../common/modals/profile-menu/profile-menu.modal';
import { NotificationMenuModal } from '../common/modals/notification-menu/notification-menu.modal';
import { NotificationService } from '../common/services/notification/notification.service';
import { NotificationModel } from '../common/models/notification/notification.model';

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

  notifications: NotificationModel[] = [];

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private router: Router,
    private dialog: MatDialog,
    private notificationService: NotificationService,
  ) {
    this.authService.isAuthenticated$.subscribe(status => {
      this.isAuthenticated = status;
    });

    this.userService.currentUser$.subscribe(user => {
      if (user && this.isAuthenticated) {
        this.username = user?.Username;
        this.userId = user?.Id;
      }
    });
  }

  ngOnInit() {
    if (!this.userId) {
      return;
    }

    this.notificationService.notification$.subscribe((notifications: NotificationModel[]) => {
      this.notifications = notifications;
    })
  }

  signin() {
    this.router.navigate(['/signin']);
  }

  signup() {
    this.router.navigate(['/signup']);
  }

  openNotification() {
    this.dialog.open(NotificationMenuModal).afterClosed().subscribe(result => {
      this.notifications = result;
    });
  }

  openProfile() {
    this.dialog.open(ProfileMenuModal, {
      data: {
        username: this.username
      }
    })
  }
}
