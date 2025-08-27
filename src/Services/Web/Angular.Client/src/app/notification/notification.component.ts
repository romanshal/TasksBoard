import { Component } from '@angular/core';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { NotificationModel } from '../common/models/notification/notification.model';
import { NotificationService } from '../common/services/notification/notification.service';
import { map, Observable, shareReplay } from 'rxjs';
import { Router } from '@angular/router';
import { UserService } from '../common/services/user/user.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { AuthStateService } from '../common/services/auth-state/auth-state.service';

@Component({
  selector: 'app-notification',
  standalone: false,
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss'
})
export class NotificationComponent {
  userId!: string;

  notifications: NotificationModel[] = [];

  pageIndex: number = 1;
  pageSize: number = 10;
  totalPages = 1;
  totalCount = 0;

  isLoading = false;

  filters: string[] = ['All', 'Unread'];
  currentFilter: string = this.filters[0];

  constructor(
    private notificationService: NotificationService,
    private router: Router,
    private userService: UserService,
    private authStateService: AuthStateService,
    private spinner: NgxSpinnerService,
  ) {
    this.spinner.show();


    this.authStateService.currentUser$.subscribe(user => {
      this.userId = user?.Id!;
    })
    
    this.getNotifications();
  }

  private getNotifications() {
    window.scrollTo(0, 0);
    if (this.currentFilter === 'All') {
      this.getAllPaginatedNotifications();
    } else if (this.currentFilter === 'Unread') {
      this.getNewPaginatedNotifications();
    }
  }

  private getAllPaginatedNotifications() {
    this.notificationService.getAllPaginatedByAccountId(this.userId, this.pageIndex, this.pageSize).subscribe(result => {
      if (result) {
        this.notifications = result.Items;

        this.pageIndex = result.PageIndex;
        this.pageSize = result.PageSize;
        this.totalPages = result.PagesCount;
        this.totalCount = result.TotalCount;

        this.spinner.hide();
        this.isLoading = true;
      }
    });
  }

  private getNewPaginatedNotifications() {
    this.notificationService.getNewPaginatedByAccountId(this.userId, this.pageIndex, this.pageSize).subscribe(result => {
      if (result) {
        this.notifications = result.Items;

        this.pageIndex = result.PageIndex;
        this.pageSize = result.PageSize;
        this.totalPages = result.PagesCount;
        this.totalCount = result.TotalCount;

        this.spinner.hide();
        this.isLoading = true;
      }
    });
  }

  changeFilter(filter: string) {
    if (this.currentFilter === filter) {
      return;
    }

    this.pageIndex = 1;
    this.currentFilter = filter;

    this.getNotifications();
  }

  goToPage(page: number | string): void {
    if (typeof page === 'number') {
      if (page === this.pageIndex) {
        return;
      }

      this.pageIndex = page;

      this.getNotifications();
    }
  }

  setAllRead() {
    if (this.notifications.length === 0 || this.notifications.filter(n => n.Read).length === 0) {
      return;
    }

    let ids = this.notifications.map(notification => notification.Id);

    this.notificationService.setRead(this.userId, ids)
      .add(this.getNotifications());

    this.getNotifications();
  }

  hasUnreadNotifications(): boolean {
    return this.notifications.length !== 0 && this.notifications.some(n => !n.Read);
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
  }

  openBoardNotice(boardId: string, boardNoticeId: string) {
    this.router.navigate(['/board/' + boardId], { queryParams: { boardnotice: boardNoticeId } });
  }

  openBoardMembers(boardId: string){
        this.router.navigate(['/board/' + boardId], { queryParams: { boardmembers: true } });
  }

  openProfile(accountId: string) {
    this.router.navigate(['/profile/' + accountId]);
  }
}
