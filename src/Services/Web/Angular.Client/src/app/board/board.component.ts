import { Component, OnInit } from '@angular/core';
import { BoardService } from '../common/services/board/board.service';
import { BoardModel } from '../common/models/board/board.model';
import { BoardNoticeService } from '../common/services/board-notice/board-notice.service';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { BoardNoticeModal } from '../common/modals/board-notice/board-notice.modal';
import { BoardNoticeModel } from '../common/models/board-notice/board-notice.model';
import { animate, query, stagger, style, transition, trigger } from '@angular/animations';

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
export class BoardComponent implements OnInit {
  boardId!: string;
  board!: BoardModel;
  private userId: any;

  notesForView: BoardNoticeModel[] = [];
  pageIndex = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  constructor(
    private boardService: BoardService,
    private boardNoticeService: BoardNoticeService,
    private sessionStorageService: SessionStorageService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
  ) {
    this.userId = this.sessionStorageService.getItem(this.sessionStorageService.userIdKey);
    this.boardId = this.route.snapshot.paramMap.get('boardid')!;
  }

  ngOnInit(): void {
    this.boardService.getBoardById(this.boardId).subscribe(result => {
      if (result) {
        this.board = result;

        this.getBoardNotices(this.pageIndex, this.pageSize);
      }
    });
  }

  private getBoardNotices(pageIndex: number, pageSize: number) {
    this.boardNoticeService.getBoardNoticesByBoardId(this.boardId, pageIndex, pageSize).subscribe(result => {
      if (result) {

        setTimeout(() => {
          this.notesForView = result.Items;
        }, 500);

        this.pageIndex = result.PageIndex;
        this.pageSize = result.PageSize;
        this.totalPages = result.PagesCount;
        this.totalCount = result.TotalCount
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

  openModal(note?: BoardNoticeModel): void {
    this.dialog.open(BoardNoticeModal, {
      data: {
        boardId: this.boardId,
        note: note
      }
    })
      .afterClosed().subscribe((result) => {
        if (result === 'success') {
          this.getBoardNotices(this.pageIndex, this.pageSize);
        }
      });
  }
}
