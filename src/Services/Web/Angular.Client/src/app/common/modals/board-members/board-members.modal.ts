import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardPermission } from '../../models/board-permission/board-permission.model';
import { BoardMemberAuthService } from '../../services/board-member-auth/board-member-auth.service';
import { BoardMemberPermissionsModal } from '../board-member-permissions/board-member-permissions.modal';
import { BoardMemberService } from '../../services/board-member/board-member.service';
import { InviteMemberModal } from '../invite-member/invite-member.modal';
import { BoardAccessRequestModel } from '../../models/board-access-request/board-access-request.model';
import { BoardMemberRequestAcceptModal } from '../board-member-request-accept/board-member-request-accept.modal';

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
  accessRequests: BoardAccessRequestModel[] = [];

  canManageMember = false;

  constructor(
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<BoardMembersModal>,
    private boardMemberAuthService: BoardMemberAuthService,
    private boardMemberService: BoardMemberService,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string, members: BoardMemberModel[], permissions: BoardPermission[], userId: string, accessRequests: BoardAccessRequestModel[] }
  ) {
    this.boardId = data.boardId;
    this.userId = data.userId;
    this.membersForView = data.members;
    this.permissions = data.permissions;
    this.accessRequests = data.accessRequests

    this.canManageMember = this.boardMemberAuthService.havePermission('manage_member');
  }

  getBoardMembers() {
    this.boardMemberService.getBoardMembersByBoardId(this.boardId).subscribe(result => {
      if (result) {
        this.membersForView = result;
      }
    });
  }

  openMemberPermission(member: BoardMemberModel) {
    this.dialog.open(BoardMemberPermissionsModal, {
      data: {
        boardId: this.boardId,
        member: member,
        permissions: this.permissions
      }
    }).afterClosed().subscribe((result) => {
      if (result === 'updated') {
        this.getBoardMembers();
      }
    });
  }

  openInvite() {
    this.dialog.open(InviteMemberModal);
  }

  openAccessRequests() {
    this.dialog.open(BoardMemberRequestAcceptModal, {
      data: {
        boardId: this.boardId,
        accessRequests: this.accessRequests
      }
    }).afterClosed().subscribe(result => {
      this.accessRequests = result;

      this.getBoardMembers();
    });
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

  closeModal(): void {
    let result = {
      members: this.membersForView,
      accessRequests: this.accessRequests
    }
    this.dialogRef.close(result);
  }
}
