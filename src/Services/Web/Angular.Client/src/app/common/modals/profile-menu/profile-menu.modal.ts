import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SessionStorageService } from '../../services/session-storage/session-storage.service';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-profile-menu',
  standalone: false,
  templateUrl: './profile-menu.modal.html',
  styleUrl: './profile-menu.modal.scss'
})
export class ProfileMenuModal {
  username!: string;

  constructor(
    private sessionService: SessionStorageService,
    private authService: AuthService,
    private dialogRef: MatDialogRef<ProfileMenuModal>,
    private router: Router,
    @Inject(MAT_DIALOG_DATA) private data: { username: string }
  ) {
    this.username = this.data.username;
  }

  signout() {
    this.authService.signout().subscribe(result => {
      this.sessionService.logout();

      window.location.href = '/signin';

      this.closeModal();
    })
  }

  openProfile() {
    this.router.navigate(['/profile']);
    this.closeModal();
  }

  openMyBoards() {
    this.closeModal();
    this.router.navigate(['/boards']);
  }

  closeModal(): void {
    this.dialogRef.close();
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
