import { Component, ElementRef, HostListener } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { debounceTime, distinctUntilChanged, of, switchMap } from 'rxjs';
import { SearchMemberService } from '../../services/search-member/search-member.service';
import { UserInfoModel } from '../../models/user/user-info.model';

@Component({
  selector: 'app-invite-member',
  standalone: false,
  templateUrl: './invite-member.modal.html',
  styleUrl: './invite-member.modal.scss'
})
export class InviteMemberModal {
  searchControl = new FormControl('');
  searchResult: UserInfoModel[] = [];

  isLoading = false;
  showDropdown = false;

  constructor(
    private dialogRef: MatDialogRef<InviteMemberModal>,
    private searchMemberService: SearchMemberService
  ) {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300), // Задержка для минимизации количества запросов
        distinctUntilChanged(),
        switchMap(value => {
          if (value && value.length >= 3) {
            this.isLoading = true;
            this.showDropdown = true;
            return this.searchMemberService.seacrhMember(value);
          } else {
            this.showDropdown = false;
            return of([]);
          }
        })
      )
      .subscribe(results => {
        this.isLoading = false;
        this.searchResult = results;
      });
  }

  selectItem(option: UserInfoModel) {
    this.searchControl.setValue(option.Username, { emitEvent: false });
    this.searchResult = [];
    this.showDropdown = false;
  }

  clearSearch() {
    this.searchControl.reset();
    this.searchResult = [];
    this.showDropdown = false;
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
