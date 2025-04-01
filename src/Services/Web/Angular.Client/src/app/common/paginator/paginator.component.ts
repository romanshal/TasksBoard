import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-paginator',
  standalone: false,
  templateUrl: './paginator.component.html',
  styleUrl: './paginator.component.scss'
})
export class PaginatorComponent implements OnInit {
  @Input() pageIndex!: number;
  @Input() pageSize!: number;
  @Input() totalPages!: number;
  @Input() totalCount!: number;

  @Output() pageChanged = new EventEmitter<number>();

  ngOnInit(): void {

  }

  get pagesToDisplay(): (number | string)[] {
    if (this.totalPages === 0) {
      return Array.from({ length: 1 }, (_, i) => 1);
    }
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

  previousPage(): void {
    if (this.pageIndex > 1) {
      this.goToPage(this.pageIndex - 1);
    }
  }

  nextPage(): void {
    if (this.pageIndex < this.totalPages) {
      this.goToPage(this.pageIndex + 1);
    }
  }

  goToPage(page: number | string): void {
    if (typeof page === 'number') {
      if (page === this.pageIndex) {
        return;
      }

      this.pageChanged.emit(page);
    }
  }
}
