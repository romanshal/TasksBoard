import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardMemberService } from '../../services/board-member/board-member.service';

@Component({
  selector: 'app-board-members',
  standalone: false,
  templateUrl: './board-members.modal.html',
  styleUrl: './board-members.modal.scss'
})
export class BoardMembersModal {
  boardId!: string;

  membersForView: BoardMemberModel[] = [];

  private openedSettingsId?: string;

  constructor(
    private dialogRef: MatDialogRef<BoardMembersModal>,
    private boardMemberService: BoardMemberService,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string }
  ) {
    this.boardId = data.boardId;

    this.getBoardMembers();
  }

  private getBoardMembers() {
    this.boardMemberService.getBoardMembersByBoardId(this.boardId).subscribe(result => {
      if (result) {
        this.membersForView = result;
        console.log(this.membersForView);
      }
    })
  }

  openSettings(id: string) {
    if (this.openedSettingsId) {
      let openedSettings = document.getElementById(this.openedSettingsId);
      if (openedSettings) {
        if (this.openedSettingsId === id) {
          openedSettings.style.display = "none";
          this.openedSettingsId = undefined;
          return;
        }

        openedSettings.style.display = "none";
      }
    }

    let settings = document.getElementById(id);
    if (!settings) {
      return;
    }

    this.openedSettingsId = id;

    settings.style.display = "block";
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

  closeModal(result?: string): void {
    this.dialogRef.close(result);
  }
}
