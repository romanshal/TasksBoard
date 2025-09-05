import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardPermission } from '../../models/board-permission/board-permission.model';
import { BoardMemberAuthService } from '../../services/board-member-auth/board-member-auth.service';
import { BoardMemberPermissionsModal } from '../board-member-permissions/board-member-permissions.modal';
import { BoardMemberService } from '../../services/board-member/board-member.service';
import { InviteMemberModal } from '../invite-member/invite-member.modal';
import { BoardAccessRequestModel } from '../../models/board-access-request/board-access-request.model';
import { BoardInviteRequestModel } from '../../models/board-invite-request/board-invite-request.model';
import { ManageBoardAccessRequestService } from '../../services/manage-board-access-request/manage-board-access-request.service';
import { Router } from '@angular/router';
import { UserService } from '../../services/user/user.service';
import { map, Observable, shareReplay } from 'rxjs';
import { ManageBoardInviteRequestService } from '../../services/manage-board-invite-request/manage-board-invite-request.service';

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
  inviteRequests: BoardInviteRequestModel[] = [];

  canManageMember = false;

  constructor(
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<BoardMembersModal>,
    private boardMemberAuthService: BoardMemberAuthService,
    private boardMemberService: BoardMemberService,
    private accessRequestsService: ManageBoardAccessRequestService,
    private inviteRequestsService: ManageBoardInviteRequestService,
    private router: Router,
    private userService: UserService,
    @Inject(MAT_DIALOG_DATA) private data:
      {
        boardId: string,
        members: BoardMemberModel[],
        permissions: BoardPermission[],
        userId: string,
        accessRequests: BoardAccessRequestModel[],
        inviteRequests: BoardInviteRequestModel[]
      }
  ) {
    this.boardId = data.boardId;
    this.userId = data.userId;
    this.membersForView = data.members;
    this.permissions = data.permissions;
    this.accessRequests = data.accessRequests;
    this.inviteRequests = data.inviteRequests;

    this.canManageMember = this.boardMemberAuthService.havePermission('manage_member');
  }

  getBoardMembers() {
    this.boardMemberService.getBoardMembersByBoardId(this.boardId).subscribe(result => {
      if (result) {
        this.membersForView = result;
      }
    });
  }

  avatarMap: { [accountId: string]: Observable<string> } = {};
  getBoardMemberAvatar(accountId: string): Observable<string> {
    if (!this.avatarMap[accountId]) {
      this.avatarMap[accountId] = this.userService.getUserAvatar(accountId)
        .pipe(
          map(image => {
            return image === '' ? 'avatar.png' : image;
          }),
          shareReplay(1)
        );
    }

    return this.avatarMap[accountId];
  }

  openMemberPermission(member: BoardMemberModel) {
    this.dialog.open(BoardMemberPermissionsModal, {
      data: {
        boardId: this.boardId,
        member: member,
        permissions: this.permissions,
        userAvatar: this.avatarMap[member.accountId]
      }
    }).afterClosed().subscribe((result) => {
      if (result === 'updated') {
        this.getBoardMembers();
      }
    });
  }

  openMemberProfile(accountId: string) {
    this.router.navigate(['/profile/' + accountId])
      .then(() => {
        setTimeout(() => {
          this.closeModal();
        }, 100);
      });
  }

  openInvite() {
    this.dialog.open(InviteMemberModal, {
      data: {
        boardId: this.boardId,
        members: this.membersForView,
        inviteRequests: this.inviteRequests,
        accessRequests: this.accessRequests
      }
    })
      .afterClosed().subscribe((result) => {
        this.inviteRequests = result;

        this.getBoardMembers();
      });
  }

  getAccessRequests(decision: boolean) {
    this.accessRequestsService.getBoardAccessRequestByBoardId(this.boardId).subscribe({
      next: (result) => {
        if (result) {
          this.accessRequests = result;

          if (decision) {
            this.getBoardMembers();
          }
        }
      }, error: (err) => {

      }
    });
  }

  getInviteRequests(){
    this.inviteRequestsService.getBoardInviteRequestByBoardId(this.boardId).subscribe({
      next: (result) => {
        this.inviteRequests = result;
      },error: (err) => {

      }
    });
  }

  resolveAccessRequest(request: BoardAccessRequestModel, decision: boolean) {
    let resolve = {
      requestId: request.id,
      decision: decision
    }

    this.accessRequestsService.resolveBoardAccessRequest(this.boardId, resolve).subscribe({
      next: (result) => {
        this.getAccessRequests(decision);
      },
      error: (error) => {

      }
    });
  }

  cancelInviteRequest(request: BoardInviteRequestModel) {
    let cancel = {
      requestId: request.id
    };

    this.inviteRequestsService.cancelInviteRequest(this.boardId, cancel).subscribe({
      next: (result) => {
        this.getInviteRequests();
      }, error: (err) => {

      }
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
      accessRequests: this.accessRequests,
      inviteRequests: this.inviteRequests
    }
    this.dialogRef.close(result);
  }
}
