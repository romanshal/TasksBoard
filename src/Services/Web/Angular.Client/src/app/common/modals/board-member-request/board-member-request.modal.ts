import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BoardAccessRequestService } from '../../services/board-access-request/board-access-request.service';
import { SessionStorageService } from '../../services/session-storage/session-storage.service';

@Component({
  selector: 'app-board-member-request',
  standalone: false,
  templateUrl: './board-member-request.modal.html',
  styleUrl: './board-member-request.modal.scss'
})
export class BoardMemberRequestModal {
  boardId!: string;

  errorMessage = '';

  success = false;
  successMessage = 'Your access request has been registered. Please, wait until someone of the participants accepts it.'

  constructor(
    private dialogRef: MatDialogRef<BoardMemberRequestModal>,
    private boardAccessService: BoardAccessRequestService,
    private sessionStorageService: SessionStorageService,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string }
  ) {
    this.boardId = data.boardId
  }

  requestAccess() {
    let user = this.sessionStorageService.getUserInfo();
    let userId = this.sessionStorageService.getItem(this.sessionStorageService.userIdKey);

    if (user === undefined || user?.Username === null || user?.Email === null || userId === null) {
      console.log('log in again');
      return;
    }

    let request = {
      accountId: userId,
      accountName: user?.Username,
      accountEmail: user?.Email
    };

    this.boardAccessService.requestBoardAccess(this.boardId, request).subscribe({
      next: result => {
        if (!result.IsError) {
          this.success = true;
        }
      },
      error: error => {
        this.errorMessage = error.error.Description;
        console.log(this.errorMessage);
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
