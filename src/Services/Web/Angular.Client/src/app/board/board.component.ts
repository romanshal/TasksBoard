import { Component, HostListener, OnInit } from '@angular/core';
import { BoardService } from '../common/services/board/board.service';
import { BoardModel } from '../common/models/board/board.model';
import { BoardNoticeService } from '../common/services/board-notice/board-notice.service';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { ActivatedRoute } from '@angular/router';
import { trigger, transition, style, animate } from '@angular/animations';
import { MatDialog } from '@angular/material/dialog';
import { AddBoardNoticeModalComponent } from '../common/modals/add-board-notice.modal/add-board-notice.modal.component';

interface NoteStyle {
  value: any;
}

export class BoardNoticeForView {
  constructor(
    public Definition: string,
    public BackgroundColor: string,
    public Rotation: string,
  ) { }
}

@Component({
  selector: 'app-board',
  standalone: false,
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss'
})
export class BoardComponent implements OnInit {
  boardId!: string;
  board!: BoardModel;
  private userId: any;

  notesForView: BoardNoticeForView[] = [];
  pageIndex = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  noteStyles: NoteStyle[] = [];

  colors: NoteStyle[] = [
    { value: '#FFEB3B' },   // Желтый
    { value: '#FF8A80' },      // Розово-красный
    { value: '#80D8FF' },   // Голубой
    { value: '#CCFF90' }       // Светло-зеленый
  ];

  rotations: NoteStyle[] = [
    { value: '6deg' },
    { value: '5.5deg' },
    { value: '5deg' },
    { value: '4.5deg' },
    { value: '4deg' },
    { value: '3.5deg' },
    { value: '3deg' },
    { value: '2.5deg' },
    { value: '2deg' },

    { value: '-2deg' },
    { value: '-2.5deg' },
    { value: '-3deg' },
    { value: '-3.5deg' },
    { value: '-4deg' },
    { value: '-4.5deg' },
    { value: '-5deg' },
    { value: '-5.5deg' },
    { value: '-6deg' },
  ]

  constructor(
    private boardService: BoardService,
    private boardNoticeService: BoardNoticeService,
    private sessionStorageService: SessionStorageService,
    private route: ActivatedRoute,
    private dialog: MatDialog
  ) {
    this.userId = this.sessionStorageService.getItem(this.sessionStorageService.userIdKey);
    this.boardId = this.route.snapshot.paramMap.get('boardid')!;
  }

  ngOnInit(): void {
    this.boardService.getBoardById(this.boardId).subscribe(result => {
      if (result) {
        this.board = result;
      }
    });

    this.getBoardNotices(this.pageIndex, this.pageSize);
  }

  private getBoardNotices(pageIndex: number, pageSise: number) {
    this.boardNoticeService.getBoardNoticesByBoardId(this.boardId, pageIndex, pageSise).subscribe(result => {
      if (result) {
        this.notesForView = result.Items.map(item => { return new BoardNoticeForView(item.Definition, this.getRandomColor(), this.getRandomRotation()) })
        this.pageIndex = result.PageIndex;
        this.pageSize = result.PageSize;
        this.totalPages = result.PagesCount;
        this.totalCount = result.TotalCount
      }
    });
  }

  getRandomColor() {
    const randomIndex = Math.floor(Math.random() * this.colors.length);
    return this.colors[randomIndex].value;
  }

  getRandomRotation() {
    const randomIndex = Math.floor(Math.random() * this.rotations.length);
    return this.rotations[randomIndex].value;
  }

  get pagesToDisplay(): (number | string)[] {
    if (this.totalPages <= 10) {
      return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }

    if (this.pageIndex <= 9) {
      return [...Array.from({ length: 9 }, (_, i) => i + 1), '...', this.totalPages];
    }

    if (this.pageIndex > 9 && this.pageIndex < this.totalPages - 1) {
      return [1, '...', this.pageIndex - 1, this.pageIndex, this.pageIndex + 1, '...', this.totalPages];
    }

    return [1, '...', ...Array.from({ length: 9 }, (_, i) => this.totalPages - 8 + i)];
  }

  goToPage(page: number | string): void {
    if (typeof page === 'number') {
      if (page === this.pageIndex) {
        return;
      }

      this.pageIndex = page;

      this.getBoardNotices(this.pageIndex, this.pageSize);
    }
  }

  previousPage(): void {
    if (this.pageIndex > 1) {
      this.pageIndex--;

      this.getBoardNotices(this.pageIndex, this.pageSize);
    }
  }

  nextPage(): void {
    if (this.pageIndex < this.totalPages) {
      this.pageIndex++;

      this.getBoardNotices(this.pageIndex, this.pageSize);
    }
  }

  openModal(content?: string): void {
    let isDisabled = content !== undefined;

    this.dialog.open(AddBoardNoticeModalComponent, {
      data: {
        borderId: this.boardId,
        disabled: isDisabled,
        content: content
      }
    })
      .afterClosed().subscribe((result) => {
        if (result === 'success') {
          this.getBoardNotices(this.pageIndex, this.pageSize);
        }
      });
  }
}
