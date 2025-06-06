import { Component, ElementRef, HostListener, Inject } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { debounceTime, distinctUntilChanged, map, Observable, of, shareReplay, switchMap } from 'rxjs';
import { SearchMemberService } from '../../services/search-member/search-member.service';
import { UserInfoModel } from '../../models/user/user-info.model';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { environment } from '../../../../environments/environment';
import { Clipboard } from '@angular/cdk/clipboard';
import { ManageBoardInviteRequestService } from '../../services/manage-board-invite-request/manage-board-invite-request.service';
import { SessionStorageService } from '../../services/session-storage/session-storage.service';
import { BoardInviteRequestModel } from '../../models/board-invite-request/board-invite-request.model';
import { BoardAccessRequestModel } from '../../models/board-access-request/board-access-request.model';
import { UserService } from '../../services/user/user.service';

@Component({
  selector: 'app-invite-member',
  standalone: false,
  templateUrl: './invite-member.modal.html',
  styleUrl: './invite-member.modal.scss'
})
export class InviteMemberModal {
  boardId!: string;

  currentUser!: UserInfoModel;

  searchControl = new FormControl('');
  searchResult: UserInfoModel[] = [];

  selectedUser?: UserInfoModel | null;

  isLoading = false;
  showDropdown = false;

  inviteLink = environment.apiUrl;

  coppied = false;

  private memberIds: Set<string>;

  private accessRequestIds: Set<string>;

  private invites: BoardInviteRequestModel[] = [];
  private inviteAccountIds: Set<string>;

  constructor(
    private dialogRef: MatDialogRef<InviteMemberModal>,
    private searchMemberService: SearchMemberService,
    private clipboard: Clipboard,
    private inviteRequestService: ManageBoardInviteRequestService,
    private sessionService: SessionStorageService,
    private userService: UserService,
    @Inject(MAT_DIALOG_DATA) private data:
      {
        boardId: string,
        members: BoardMemberModel[],
        inviteRequests: BoardInviteRequestModel[],
        accessRequests: BoardAccessRequestModel[]
      }
  ) {
    this.boardId = data.boardId;

    this.memberIds = new Set(data.members.map(member => member.AccountId));
    this.accessRequestIds = new Set(data.accessRequests.map(request => request.AccountId))
    this.invites = data.inviteRequests;
    this.inviteAccountIds = new Set(this.invites.map(request => request.ToAccountId));

    this.currentUser = this.sessionService.getUserInfo()!;

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
        this.searchResult = this.getDifference(results);
      });
  }

  getInviteRequests() {
    this.inviteRequestService.getBoardInviteRequestByBoardId(this.boardId).subscribe(result => {
      if (result) {
        this.invites = result;
        this.inviteAccountIds = new Set(result.map(request => request.ToAccountId));
      }
    })
  }

  selectItem(option: UserInfoModel) {
    // this.searchControl.setValue(option.Username, { emitEvent: false });
    this.searchControl.reset();
    this.selectedUser = option;
    this.searchResult = [];
    this.showDropdown = false;
  }

  clearSearch() {
    this.searchControl.reset();
    this.searchResult = [];
    this.showDropdown = false;
  }

  clearSelectedUser() {
    this.selectedUser = null;
  }

  private getDifference(searchResult: UserInfoModel[]) {
    // Фильтруем первый массив, исключая объекты, id которых присутствует во втором массиве
    return searchResult
      .filter(result => !this.memberIds.has(result.Id) && !this.inviteAccountIds.has(result.Id) && !this.accessRequestIds.has(result.Id));
  }

  copyLink() {
    this.coppied = true;
    this.clipboard.copy(this.inviteLink);

    setTimeout(() => {
      this.coppied = false;
    }, 3000);
  }

  inviteMember() {
    if (!this.selectedUser) {
      return;
    }

    let invite = {
      fromAccountId: this.currentUser.Id,
      fromAccountNAme: this.currentUser.Username,
      toAccountId: this.selectedUser.Id,
      toAccountName: this.selectedUser.Username,
      toAccountEmail: this.selectedUser.Email
    }

    this.inviteRequestService.createInviteRequest(this.boardId, invite).subscribe(result => {
      if (result) {
        this.selectedUser = null;

        this.getInviteRequests();
      }
    })
  }

    avatarMap: { [accountId: string]: Observable<string> } = {};
    getBoardMemberAvatar(accountId: string): Observable<string> {
      if (!this.avatarMap[accountId]) {
        this.avatarMap[accountId] = this.userService.getUserAvatar(accountId)
          .pipe(
            map(image => {
              // Если полученная строка пустая, возвращаем путь к дефолтной картинке
              return image === '' ? 'avatar.png' : image;
            }),
            shareReplay(1)
          );
      }
  
      return this.avatarMap[accountId];
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
    this.dialogRef.close(this.invites);
  }
}
