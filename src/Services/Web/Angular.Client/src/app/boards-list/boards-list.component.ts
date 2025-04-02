import { Component, OnInit } from '@angular/core';
import { BoardModel } from '../common/models/board/board.model';
import { BoardService } from '../common/services/board/board.service';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { Response } from '../common/models/response/response.model';
import { Router } from '@angular/router';
import { animate, query, stagger, style, transition, trigger } from '@angular/animations';

const listAnimation = trigger('listAnimation', [
  transition('* <=> *', [
    query('.boards-list__item:enter',
      [style({ opacity: 0 }), stagger('300ms', animate('1s ease', style({ opacity: 1 })))],
      { optional: true }
    ),
    query(':leave',
      animate('300ms', style({ opacity: 0 })),
      { optional: true }
    )
  ])
]);

@Component({
  selector: 'app-boards-list',
  standalone: false,
  templateUrl: './boards-list.component.html',
  styleUrl: './boards-list.component.scss',
  animations: [listAnimation],
})
export class BoardsListComponent implements OnInit {
  boards: BoardModel[] = [];

  private userId: any;

  pageIndex = 1;
  pageSize = 5;
  totalPages = 1;
  totalCount = 0;

  constructor(
    private boardService: BoardService,
    private sessionService: SessionStorageService,
    private router: Router
  ) {
    this.userId = this.sessionService.getItem(this.sessionService.userIdKey);
  }

  ngOnInit(): void {
    this.getBoards(this.pageIndex, this.pageSize);
  }

  getBoards(pageIndex: number, pageSize: number) {
    this.boardService.getBoardsByUserId(this.userId!, pageIndex, pageSize).subscribe({
      next: (result) => {
        setTimeout(() => {
          this.boards = result.Items;
        }, 300)


        this.pageIndex = result.PageIndex;
        this.pageSize = result.PageSize;
        this.totalPages = result.PagesCount;
        this.totalCount = result.TotalCount
      },
      error: Response => {

      },
      complete: () => {
      }
    });
  }

  goToPage(page: number | string): void {
    if (typeof page === 'number') {
      if (page === this.pageIndex) {
        return;
      }

      this.pageIndex = page;
      this.boards = [];

      this.getBoards(this.pageIndex, this.pageSize);
    }
  }

  openBoard(boardId: string) {
    this.router.navigate(['/board/' + boardId]);
  }
}
