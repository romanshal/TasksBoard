import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SessionStorageService } from '../../services/session-storage/session-storage.service';

@Component({
  selector: 'app-profile-menu',
  standalone: false,
  templateUrl: './profile-menu.modal.component.html',
  styleUrl: './profile-menu.modal.component.scss'
})
export class ProfileMenuModalComponent {
  username!: string;

  constructor(
    private sessionService: SessionStorageService,
    private dialogRef: MatDialogRef<ProfileMenuModalComponent>,
    @Inject(MAT_DIALOG_DATA) private data: { username: string }
  ) {
    this.username = this.data.username;
  }

  signout() {
    this.sessionService.logout();

    window.location.href = '/login';
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
