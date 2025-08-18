import { Component, OnDestroy, OnInit } from '@angular/core';
import { BoardService } from '../common/services/board/board.service';
import { BoardModel } from '../common/models/board/board.model';
import { BoardNoticeService } from '../common/services/board-notice/board-notice.service';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { BoardNoticeModal } from '../common/modals/board-notice/board-notice.modal';
import { BoardNoticeModel } from '../common/models/board-notice/board-notice.model';
import { animate, query, stagger, style, transition, trigger } from '@angular/animations';
import { BoardMembersModal } from '../common/modals/board-members/board-members.modal';
import { BoardInfoModal } from '../common/modals/board-info/board-info.modal';
import { BoardMemberModel } from '../common/models/board-member/board-member.model';
import { BoardPermission } from '../common/models/board-permission/board-permission.model';
import { BoardPermissionService } from '../common/services/board-permission/board-permission.service';
import { BoardMemberAuthService } from '../common/services/board-member-auth/board-member-auth.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ChatService } from '../common/services/chat/chat.service';
import { BoardAccessRequestService } from '../common/services/board-access-request/board-access-request.service';
import { BoardMemberService } from '../common/services/board-member/board-member.service';
import { ManageBoardAccessRequestService } from '../common/services/manage-board-access-request/manage-board-access-request.service';
import { BoardAccessRequestModel } from '../common/models/board-access-request/board-access-request.model';
import { BoardInviteRequestModel } from '../common/models/board-invite-request/board-invite-request.model';
import { ManageBoardInviteRequestService } from '../common/services/manage-board-invite-request/manage-board-invite-request.service';

const listAnimation = trigger('listAnimation', [
  transition('* <=> *', [
    query(':enter',
      [style({ opacity: 0 }), stagger('200ms', animate('700ms ease', style({ opacity: 1 })))],
      { optional: true }
    ),
    query(':leave',
      animate('300ms', style({ opacity: 0 })),
      { optional: true }
    )
  ])
]);

@Component({
  selector: 'app-board',
  standalone: false,
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss',
  animations: [listAnimation]
})
export class BoardComponent implements OnInit, OnDestroy {
  boardId!: string;
  board!: BoardModel;
  public userId: any;

  notesForView: BoardNoticeModel[] = [];
  pageIndex = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  boardMembers: BoardMemberModel[] = [];

  accessRequests: BoardAccessRequestModel[] = [];

  inviteRequests: BoardInviteRequestModel[] = [];

  boardPermissions: BoardPermission[] = [];

  canAddNotices = false;

  currentMember!: BoardMemberModel;

  isLoading = false;

  constructor(
    private boardService: BoardService,
    private boardNoticeService: BoardNoticeService,
    private boardPermissionService: BoardPermissionService,
    private sessionStorageService: SessionStorageService,
    private boardMemberAuthService: BoardMemberAuthService,
    private boardAccessRequestService: ManageBoardAccessRequestService,
    private boardInviteReqquestService: ManageBoardInviteRequestService,
    private boardMemberService: BoardMemberService,
    private chatService: ChatService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private spinner: NgxSpinnerService
  ) {
    this.spinner.show();

    this.userId = this.sessionStorageService.getItem(this.sessionStorageService.userIdKey);
    this.boardId = this.route.snapshot.paramMap.get('boardid')!;
    this.getBoard();
  }

  ngOnInit(): void {
    this.getPermissions();
  }

  ngOnDestroy() {
    this.chatService.stop();
  }

  private getBoard() {
    this.boardService.getBoardById(this.boardId).subscribe(result => {
      if (result) {
        this.board = result;

        this.boardMemberService.getBoardMembersByBoardId(this.boardId).subscribe(result => {
          if (result) {
            this.boardMembers = result;

            this.currentMember = this.boardMembers.find(member => member.AccountId === this.userId)!

            this.boardMemberAuthService.initialize(this.board, this.currentMember);

            this.canAddNotices = this.boardMemberAuthService.havePermission('manage_notice');
          }
        });

        this.boardAccessRequestService.getBoardAccessRequestByBoardId(this.boardId).subscribe(result => {
          if (result) {
            this.accessRequests = result;
          }
        });

        this.boardInviteReqquestService.getBoardInviteRequestByBoardId(this.boardId).subscribe(result => {
          if (result) {
            this.inviteRequests = result;
          }
        });

        this.getBoardNotices(this.pageIndex, this.pageSize);

        if (this.route.snapshot.queryParamMap.get('boardnotice')) {
          let noticeId = this.route.snapshot.queryParamMap.get('boardnotice')!;
          this.openBoardNoticeModalById(noticeId);
        }

        if (this.route.snapshot.queryParamMap.get('boardmembers')) {
          this.openMembersModal();
        }
      }
    });
  }

  private getBoardNotices(pageIndex: number, pageSize: number) {
    this.boardNoticeService.getBoardNoticesByBoardId(this.boardId, pageIndex, pageSize).subscribe(result => {
      if (result) {

        setTimeout(() => {
          this.spinner.hide();
          this.isLoading = true;

          this.notesForView = result.Items;
        }, 500);

        this.pageIndex = result.PageIndex;
        this.pageSize = result.PageSize;
        this.totalPages = result.PagesCount;
        this.totalCount = result.TotalCount;
      }
    });
  }

  private openBoardNoticeModalById(boardNoticeId: string) {
    this.boardNoticeService.getBoardNoticeById(this.boardId, boardNoticeId).subscribe(result => {
      if (result) {
        this.openNoticeModal(result);
      }
    })
  }

  private getPermissions() {
    this.boardPermissionService.getPermissions().subscribe(result => {
      if (result) {
        this.boardPermissions = result;
      }
    });
  }

  goToPage(page: number | string): void {
    if (typeof page === 'number') {
      if (page === this.pageIndex) {
        return;
      }

      this.pageIndex = page;
      this.notesForView = [];

      this.getBoardNotices(this.pageIndex, this.pageSize);
    }
  }

  openNoticeModal(note?: BoardNoticeModel): void {
    this.dialog.open(BoardNoticeModal, {
      data: {
        boardId: this.boardId,
        note: note,
      }
    })
      .afterClosed().subscribe((result) => {
        if (result === 'success') {
          this.getBoardNotices(this.pageIndex, this.pageSize);
        }
      });
  }

  openMembersModal() {
    this.dialog.open(BoardMembersModal, {
      data: {
        boardId: this.boardId,
        members: this.boardMembers,
        permissions: this.boardPermissions,
        userId: this.userId,
        accessRequests: this.accessRequests,
        inviteRequests: this.inviteRequests
      }
    })
      .afterClosed().subscribe((result) => {
        this.boardMembers = result.members;
        this.accessRequests = result.accessRequests;
        this.inviteRequests = result.inviteRequests;
      });
  }

  openInfoModal() {
    this.dialog.open(BoardInfoModal, {
      data: {
        board: this.board,
        isOwner: this.board.OwnerId === this.userId,
      }
    })
      .afterClosed().subscribe((result) => {
        if (result === 'updated') {
          this.getBoard();
        }
      });
  }
}
