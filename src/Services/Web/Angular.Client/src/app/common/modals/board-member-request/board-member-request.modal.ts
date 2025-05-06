import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-board-member-request',
  standalone: false,
  templateUrl: './board-member-request.modal.html',
  styleUrl: './board-member-request.modal.scss'
})
export class BoardMemberRequestModal {
  boardId!: string;

  constructor(
    private dialogRef: MatDialogRef<BoardMemberRequestModal>,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string }
  ) {
    this.boardId = data.boardId
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
