import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NotificationService } from '../../services/notification/notification.service';
import { SessionStorageService } from '../../services/session-storage/session-storage.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { NotificationModel } from '../../models/notification/notification.model';
import { Router } from '@angular/router';
import { map, Observable, shareReplay } from 'rxjs';
import { UserService } from '../../services/user/user.service';

@Component({
  selector: 'app-notification',
  standalone: false,
  templateUrl: './notification-menu.modal.html',
  styleUrl: './notification-menu.modal.scss'
})
export class NotificationMenuModal {
  userId!: string;

  notifications: NotificationModel[] = [];
  pageIndex = 1;
  pageSize = 10;

  isLoading = false;

  constructor(
    private dialogRef: MatDialogRef<NotificationMenuModal>,
    private sessionService: SessionStorageService,
    private notificationService: NotificationService,
    private spinner: NgxSpinnerService,
    private router: Router,
    private userService: UserService
  ) {
    this.spinner.show();

    this.notificationService.notification$.subscribe((notifications: NotificationModel[]) => {
      this.notifications = notifications;
    })

    this.userId = this.sessionService.getItem(this.sessionService.userIdKey)!;

    this.isLoading = true;
    this.spinner.hide();
  }

  getCreatedAtDifference(createdAt: Date): string {
    const parsedDate = new Date(createdAt);
    if (isNaN(parsedDate.getTime())) {
      return "Invalid date";
    }

    const now = new Date();
    const diffMilliseconds = now.getTime() - parsedDate.getTime();
    const diffSeconds = diffMilliseconds / 1000;
    const diffMinutes = diffSeconds / 60;
    const diffHours = diffMinutes / 60;
    const diffDays = diffHours / 24;
    const diffWeeks = diffDays / 7;
    const diffMonths = diffDays / 30;
    const diffYears = diffDays / 365;

    if (diffSeconds < 60) {
      const seconds = Math.floor(diffSeconds);
      return seconds === 1 ? `${seconds} second ago` : `${seconds} seconds ago`;
    } else if (diffMinutes < 60) {
      const minutes = Math.floor(diffMinutes);
      return minutes === 1 ? `${minutes} minute ago` : `${minutes} minutes ago`;
    } else if (diffHours < 24) {
      const hours = Math.floor(diffHours);
      return hours === 1 ? `${hours} hour ago` : `${hours} hours ago`;
    } else if (diffDays < 7) {
      const days = Math.floor(diffDays);
      return days === 1 ? `${days} day ago` : `${days} days ago`;
    } else if (diffWeeks < 4) {
      const weeks = Math.floor(diffWeeks);
      return weeks === 1 ? `${weeks} week ago` : `${weeks} weeks ago`;
    } else if (diffMonths < 12) {
      const months = Math.floor(diffMonths);
      return months === 1 ? `${months} month ago` : `${months} months ago`;
    } else {
      const years = Math.floor(diffYears);
      return years === 1 ? `${years} year ago` : `${years} years ago`;
    }
  }

  avatarMap: { [accountId: string]: Observable<string> } = {};
  getAccountAvatar(accountId: string): Observable<string> {
    if (!this.avatarMap[accountId]) {
      this.avatarMap[accountId] = this.userService.getUserAvatar(accountId)
        .pipe(
          map(image => {
            // Если полученная строка пустая, возвращаем путь к дефолтной картинке
            return image === '' ? 'avatar.png' : image;
          }),
          shareReplay(1)
        );
    }

    return this.avatarMap[accountId];
  }

  openBoard(boardId: string) {
    this.router.navigate(['/board/' + boardId]);

    this.closeModal();
  }

  openBoardNotice(boardId: string, boardNoticeId: string) {
    this.router.navigate(['/board/' + boardId], { queryParams: { boardnotice: boardNoticeId } });
    this.closeModal();
  }

  openBoardMembers(boardId: string) {
    this.router.navigate(['/board/' + boardId], { queryParams: { boardmembers: true } });
    
    this.closeModal();
  }

  openProfile(accountId: string) {
    this.router.navigate(['/profile/' + accountId]);

    this.closeModal();
  }

  openNotifications() {
    this.router.navigate(['/notification'])

    this.closeModal();
  }

  setRead(notificationId: string) {
    if (!notificationId) {
      return;
    }

    let ids = [notificationId];

    this.notificationService.setRead(this.userId, ids);
  }

  setAllRead() {
    if (!this.notifications) {
      return;
    }

    let ids = this.notifications.map(notification => notification.Id);

    this.notificationService.setRead(this.userId, ids);
  }

  closeModal(): void {
    this.dialogRef.close(this.notifications);
  }

  onContainerClick(event: MouseEvent): void {
    // Если кликнули по затемнённой области вне модального окна
    if ((event.target as HTMLElement).classList.contains('modal-container')) {
      this.closeModal();
    }
  }

  // Закрытие модального окна по клавише ESC
  @HostListener('document:keydown.escape', ['$event'])
  onEscapePress(event: KeyboardEvent) {
    this.closeModal();
  }
}
