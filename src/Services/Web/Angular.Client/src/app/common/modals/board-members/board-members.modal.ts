import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardMemberService } from '../../services/board-member/board-member.service';
import { BoardPermission } from '../../models/board-permission/board-permission.model';
import { BoardPermissionService } from '../../services/board-permission/board-permission.service';

@Component({
  selector: 'app-board-members',
  standalone: false,
  templateUrl: './board-members.modal.html',
  styleUrl: './board-members.modal.scss'
})
export class BoardMembersModal {
  boardId!: string;
  userId!: string;
  permissions: BoardPermission[] = [];
  membersForView: BoardMemberModel[] = [];

  private openedSettingsId?: string;

  constructor(
    private dialogRef: MatDialogRef<BoardMembersModal>,
    private boardPermissionService: BoardPermissionService,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string, members: BoardMemberModel[], permissions: BoardPermission[], userId: string }
  ) {
    this.boardId = data.boardId;
    this.userId = data.userId;
    this.membersForView = data.members;
    this.permissions = data.permissions;
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

  isPermissionsCheck(member: BoardMemberModel, permissionId: string) {
    if (member.Permissions.findIndex(p => p.BoardPermissionId === permissionId) !== -1) {
      return true;
    } else {
      return false;
    }
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
