import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-paginator-new',
  standalone: false,
  templateUrl: './paginator-new.component.html',
  styleUrl: './paginator-new.component.scss'
})
export class PaginatorNewComponent implements OnInit {
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

    if (this.totalPages <= 5) {
      return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }

    if (this.pageIndex <= 4) {
      return [...Array.from({ length: 4 }, (_, i) => i + 1), '...', this.totalPages];
    }

    if (this.pageIndex > 4 && this.pageIndex < this.totalPages - 1) {
      return [1, '...', this.pageIndex - 1, this.pageIndex, this.pageIndex + 1, '...', this.totalPages];
    }

    return [1, '...', ...Array.from({ length: 4 }, (_, i) => this.totalPages - 3 + i)];
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
