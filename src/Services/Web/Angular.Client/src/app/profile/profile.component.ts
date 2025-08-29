import { Component, OnInit } from '@angular/core';
import { UserInfoModel } from '../common/models/user/user-info.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../common/services/user/user.service';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { OperationResultMessageModal } from '../common/modals/operation-result-message/operation-result-message.modal';
import { ProfileAvatarModal } from '../common/modals/profile-avatar/profile-avatar.modal';
import { BoardInviteRequestService } from '../common/services/board-invite-request/board-invite-request.service';
import { BoardInviteRequestModel } from '../common/models/board-invite-request/board-invite-request.model';
import { forkJoin, map, Observable, shareReplay, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { BoardAccessRequestModel } from '../common/models/board-access-request/board-access-request.model';
import { BoardAccessRequestService } from '../common/services/board-access-request/board-access-request.service';
import { AuthService } from '../common/services/auth/auth.service';
import { AuthSessionService } from '../common/services/auth-session/auth-session.service';

@Component({
  selector: 'app-profile',
  standalone: false,
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  user!: UserInfoModel;
  isCurrentUser = false;
  userAvatarSrc: string | undefined = '';

  generalSettingsForm!: FormGroup;
  passwordSettingsForm!: FormGroup;

  generalSettingsFormSubmitted = false;
  passwordSettingsFormSubmitted = false;

  showCurrentPassword = false;
  showNewPassword = false;

  updatetUserInfoErrorMessage = '';
  changeUserPasswordErrorMessage = '';

  boardInviteRequests: BoardInviteRequestModel[] = [];
  boardAccessRequests: BoardAccessRequestModel[] = [];

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private authSessionService: AuthSessionService,
    private route: ActivatedRoute,
    private boardInviteRequestService: BoardInviteRequestService,
    private boardAccessRequestService: BoardAccessRequestService,
    private dialog: MatDialog,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.resolveUserContext();
  }

  private resolveUserContext(): void {
    const routeUserId = this.route.snapshot.paramMap.get('userid');
    const currentUser = this.authSessionService.getCurrentUser()!;
    this.isCurrentUser = !routeUserId || routeUserId === currentUser.Id;

    const userIdToLoad = this.isCurrentUser ? currentUser.Id : routeUserId!;

    if (this.isCurrentUser) {
      this.loadBoardRequests(currentUser.Id);
    }

    this.loadUserData(userIdToLoad);
  }

  private loadUserData(userId: string): void {
    this.userService.getUserInfo(userId)
      .pipe(
        tap(user => this.user = user),
        switchMap(() => this.userService.getUserAvatar(userId)),
        tap(avatar => this.userAvatarSrc = avatar || undefined),
        takeUntil(this.destroy$)
      )
      .subscribe({
        complete: () => {
          this.initGeneralSettingsForm();
          this.initPasswordSettingsForm();
        }
      });
  }

  private loadBoardRequests(userId: string): void {
    forkJoin({
      invites: this.boardInviteRequestService.getByAccountId(userId),
      access: this.boardAccessRequestService.getByAccountId(userId)
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe(({ invites, access }) => {
        this.boardInviteRequests = invites || [];
        this.boardAccessRequests = access || [];
      });
  }

  private initGeneralSettingsForm(): void {
    this.generalSettingsForm = this.fb.group({
      username: [{ value: this.user.Username, disabled: !this.isCurrentUser }, [Validators.required, Validators.minLength(3), Validators.maxLength(20)]],
      email: [{ value: this.user.Email, disabled: !this.isCurrentUser }, [Validators.required, Validators.email]],
      firstName: [{ value: this.user.Firstname, disabled: !this.isCurrentUser }],
      surname: [{ value: this.user.Surname, disabled: !this.isCurrentUser }],
    });
  }

  private initPasswordSettingsForm(): void {
    this.passwordSettingsForm = this.fb.group({
      currentPassword: [{ value: '', disabled: !this.isCurrentUser }, [Validators.required, Validators.minLength(8)]],
      newPassword: [{ value: '', disabled: !this.isCurrentUser }, [Validators.required, Validators.minLength(8)]]
    });
  }

  openProfileAvater() {
    this.dialog.open(ProfileAvatarModal, {
      data: {
        userId: this.user.Id,
        avatar: this.userAvatarSrc
      }
    })
      .afterClosed().subscribe(result => {
        if (result) {
          this.userAvatarSrc = result;
        }
      })
  }

  signout() {
    this.authService.signout().subscribe({
      next: () => {
        window.location.href = '/signin';
      }
    })
  }

  updateUserInfo() {
    if (!this.isCurrentUser) {
      return;
    }

    this.generalSettingsFormSubmitted = true;

    if (this.generalSettingsForm.invalid) {
      return;
    }

    this.userService.updateUserInfo(this.user!.Id, this.generalSettingsForm.value).subscribe(result => {
      if (result) {
        this.user = result;

        this.initGeneralSettingsForm();

        this.dialog.open(OperationResultMessageModal, {
          data: {
            isSuccess: true,
            message: 'Profile was successfully updated'
          }
        });
      }
    });
  }

  changeUserPassword() {
    if (!this.isCurrentUser) {
      return;
    }

    this.passwordSettingsFormSubmitted = true;

    if (this.passwordSettingsForm.invalid) {
      return;
    }

    this.userService.changeUserPassword(this.user!.Id, this.passwordSettingsForm.value).subscribe(result => {
      if (result) {
        this.passwordSettingsForm.reset();

        this.dialog.open(OperationResultMessageModal, {
          data: {
            isSuccess: true,
            message: 'Password was successfully changed'
          }
        })
      }
    }, error => {
      this.changeUserPasswordErrorMessage = error.error.Description;
    })
  }

  avatarMap: { [accountId: string]: Observable<string> } = {};
  getAccountAvatar(accountId: string): Observable<string> {
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

  resolveInviteRequest(request: BoardInviteRequestModel, decision: boolean) {
    if (!request) {
      return;
    }

    let resolve = {
      requestId: request.Id,
      decision: decision
    }

    this.boardInviteRequestService.resolveBoardInviteRequest(request.BoardId, resolve).subscribe(result => {
      if (result) {
        this.boardInviteRequests = this.boardInviteRequests.filter(req => req.Id !== request.Id);
      }
    })
  }

  cancelAccessRequest(request: BoardAccessRequestModel) {
    if (!request) {
      return;
    }

    let body = {
      requestId: request.Id
    };

    this.boardAccessRequestService.cancelAccessRequest(body).subscribe(result => {
      if (result) {
        this.boardAccessRequests = this.boardAccessRequests.filter(req => req.Id !== request.Id);
      }
    });
  }
}
