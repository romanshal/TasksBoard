import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BoardAccessRequestService } from '../../services/board-access-request/board-access-request.service';
import { UserInfoModel } from '../../models/user/user-info.model';
import { AuthSessionService } from '../../services/auth-session/auth-session.service';

@Component({
  selector: 'app-board-member-request',
  standalone: false,
  templateUrl: './board-member-request.modal.html',
  styleUrl: './board-member-request.modal.scss'
})
export class BoardMemberRequestModal {
  boardId!: string;

  currentUser?: UserInfoModel | null;

  errorMessage = '';

  success = false;
  successMessage = 'Your access request has been registered. Please, wait until someone of the participants accepts it.'

  constructor(
    private dialogRef: MatDialogRef<BoardMemberRequestModal>,
    private boardAccessService: BoardAccessRequestService,
    private authSessionService: AuthSessionService,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string }
  ) {
    this.boardId = data.boardId;

    this.authSessionService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  requestAccess() {
    if (this.currentUser === null || this.currentUser?.username === null || this.currentUser?.email === null || this.currentUser?.id === null) {
      return;
    }

    let request = {
      accountId: this.currentUser?.id,
    };

    this.boardAccessService.requestBoardAccess(this.boardId, request).subscribe({
      next: () => {
        this.success = true;
      },
      error: error => {
        this.errorMessage = error.error.Description;
      }
    });
  }

  closeModal(result?: string): void {
    this.dialogRef.close(result);
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
