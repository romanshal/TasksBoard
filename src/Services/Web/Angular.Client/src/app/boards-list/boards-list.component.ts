import { Component, OnInit } from '@angular/core';
import { BoardService } from '../common/services/board/board.service';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { Router } from '@angular/router';
import { animate, query, stagger, style, transition, trigger } from '@angular/animations';
import { MatDialog } from '@angular/material/dialog';
import { BoardInfoModal } from '../common/modals/board-info/board-info.modal';
import { debounceTime, distinctUntilChanged, of, Subject, switchMap } from 'rxjs';
import { BoardMemberRequestModal } from '../common/modals/board-member-request/board-member-request.modal';
import { BoardForViewModel } from '../common/models/board/board-for-view.model';
import { NgxSpinnerService } from 'ngx-spinner';
import { UserService } from '../common/services/user/user.service';

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
  boards: BoardForViewModel[] = [];

  private userId: any;

  private publicBoards = false;

  pageIndex = 1;
  pageSize = 6;
  totalPages = 1;
  totalCount = 0;

  searchString = '';
  private searchSubject: Subject<string> = new Subject();

  isLoading = false;

  constructor(
    private boardService: BoardService,
    private userService: UserService,
    private router: Router,
    private dialog: MatDialog,
    private spinner: NgxSpinnerService
  ) {
    this.spinner.show();

    this.userService.currentUser$.subscribe(user => {
      this.userId = user?.Id;
    });

    this.publicBoards = this.router.url.includes('public');
  }

  ngOnInit(): void {
    this.getBoards(this.searchString, false, this.pageIndex, this.pageSize);
    this.subscribeToSearchChanges();
  }

  onSearchChange() {
    this.searchSubject.next(this.searchString);
  }

  subscribeToSearchChanges(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(term => {
        this.pageIndex = 1;
        return of(this.getBoards(term, true, this.pageIndex, this.pageSize));
      })
    ).subscribe(data => {

    });
  }

  getBoards(query: string, delay: boolean, pageIndex: number, pageSize: number) {
    if (this.publicBoards) {
      this.boardService.getPublicBoards(pageIndex, pageSize).subscribe({
        next: (result) => {
          if (delay) {
            setTimeout(() => {
              this.boards = result.Items;
            }, 300)
          } else {
            this.boards = result.Items;
          }

          this.pageIndex = result.PageIndex;
          this.pageSize = result.PageSize;
          this.totalPages = result.PagesCount;
          this.totalCount = result.TotalCount
        },
        error: Response => {

        },
        complete: () => {
          this.spinner.hide();
          this.isLoading = true;
        }
      })
    } else {
      this.boardService.getBoardsByUserId(query, this.userId!, pageIndex, pageSize).subscribe({
        next: (result) => {
          if (delay) {
            setTimeout(() => {
              this.boards = result.Items;
            }, 300)
          } else {
            this.boards = result.Items;
          }

          this.pageIndex = result.PageIndex;
          this.pageSize = result.PageSize;
          this.totalPages = result.PagesCount;
          this.totalCount = result.TotalCount
        },
        error: Response => {

        },
        complete: () => {
          this.spinner.hide();
          this.isLoading = true;
        }
      });
    }
  }

  getImageUrl(board: BoardForViewModel): string | null {
    if (!board.Image) {
      return null;
    }

    const mimeTypeMap: Record<string, string> = {
      '.png': 'image/png',
      '.jpg': 'image/jpeg',
      '.jpeg': 'image/jpeg',
      '.gif': 'image/gif'
    };

    const mimeType = mimeTypeMap[board.ImageExtension] || 'application/octet-stream';
    let base64Image = board.Image;

    if (!base64Image.startsWith('data:')) {
      base64Image = `data:${mimeType};base64,${base64Image}`;
    }

    return base64Image;
  }

  goToPage(page: number | string): void {
    if (typeof page === 'number') {
      if (page === this.pageIndex) {
        return;
      }

      this.pageIndex = page;
      this.boards = [];

      this.getBoards(this.searchString, true, this.pageIndex, this.pageSize);
    }
  }

  openBoard(board: BoardForViewModel) {
    if (board.IsMember) {
      this.router.navigate(['/board/' + board.Id]);

      return;
    }

    this.dialog.open(BoardMemberRequestModal, {
      data: {
        boardId: board.Id
      }
    });
  }

  openCreateModal() {
    this.dialog.open(BoardInfoModal, {
      data: {
        isOwner: true
      }
    })
      .afterClosed().subscribe((result) => {
        if (result) {
          this.router.navigate(['/board/' + result]);
        }
      });
  }
}
