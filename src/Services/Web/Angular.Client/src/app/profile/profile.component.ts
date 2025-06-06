import { Component, OnInit } from '@angular/core';
import { UserInfoModel } from '../common/models/user/user-info.model';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../common/services/user/user.service';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { OperationResultMessageModal } from '../common/modals/operation-result-message/operation-result-message.modal';
import { ProfileAvatarModal } from '../common/modals/profile-avatar/profile-avatar.modal';
import { BoardInviteRequestService } from '../common/services/board-invite-request/board-invite-request.service';
import { BoardInviteRequestModel } from '../common/models/board-invite-request/board-invite-request.model';
import { map, Observable, shareReplay } from 'rxjs';
import { BoardAccessRequestModel } from '../common/models/board-access-request/board-access-request.model';
import { BoardAccessRequestService } from '../common/services/board-access-request/board-access-request.service';
import { AuthService } from '../common/services/auth/auth.service';

@Component({
  selector: 'app-profile',
  standalone: false,
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  userId!: string;
  user!: UserInfoModel;
  isCurrentUser = false;
  userAvatarSrc = '';

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

  constructor(
    private fb: FormBuilder,
    private sessionService: SessionStorageService,
    private userService: UserService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private boardInviteRequestService: BoardInviteRequestService,
    private boardAccessRequestService: BoardAccessRequestService,
    private dialog: MatDialog,
  ) {
    if (this.route.snapshot.paramMap.get('userid') === null) {
      this.userId = this.sessionService.getItem(this.sessionService.userIdKey)!;
      this.isCurrentUser = true;
    } else {
      this.userId = this.route.snapshot.paramMap.get('userid')!;

      if (this.userId === this.sessionService.getItem(this.sessionService.userIdKey)) {
        this.isCurrentUser = true;
      }
    }

    this.userService.getUserInfo(this.userId).subscribe(result => {
      this.user = result;

      this.userService.getUserAvatar(this.userId).subscribe(result => {
        if (result) {
          this.userAvatarSrc = result;
        }
      });

      this.initGeneralSettingsForm();
      this.initPasswordSettingsForm();
    });
  }

  ngOnInit(): void {
    if (this.isCurrentUser) {
      this.getBoardInviteRequests();
      this.getBoardAccessRequests();
    }
  }

  getBoardInviteRequests() {
    this.boardInviteRequestService.getByAccountId(this.userId).subscribe(result => {
      if (result) {
        this.boardInviteRequests = result;
      }
    })
  }

  getBoardAccessRequests() {
    this.boardAccessRequestService.getByAccountId(this.userId).subscribe(result => {
      if (result) {
        console.log(result);
        this.boardAccessRequests = result;
      }
    })
  }

  initGeneralSettingsForm() {
    this.generalSettingsForm = this.fb.group({
      username: [{ value: this.user!.Username, disabled: !this.isCurrentUser }, [Validators.required, Validators.minLength(3), Validators.maxLength(20)],],
      email: [{ value: this.user!.Email, disabled: !this.isCurrentUser }, [Validators.required, Validators.email]],
      firstName: [{ value: this.user!.Firstname, disabled: !this.isCurrentUser }],
      surname: [{ value: this.user!.Surname, disabled: !this.isCurrentUser }],
    });
  }

  initPasswordSettingsForm() {
    this.passwordSettingsForm = this.fb.group({
      currentPassword: [{ value: '', disabled: !this.isCurrentUser }, [Validators.required, Validators.minLength(8)]],
      newPassword: [{ value: '', disabled: !this.isCurrentUser }, [Validators.required, Validators.minLength(8)]]
    });
  }

  openProfileAvater() {
    this.dialog.open(ProfileAvatarModal, {
      data: {
        userId: this.userId,
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
    this.authService.signout().subscribe(result => {
      this.sessionService.logout();

      window.location.href = '/signin';
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
        this.userService.getUserInfo(result).subscribe(result => {
          this.sessionService.setUserInfo(result);
          this.user = this.sessionService.getUserInfo()!;

          this.initGeneralSettingsForm();

          this.dialog.open(OperationResultMessageModal, {
            data: {
              isSuccess: true,
              message: 'Profile was successfully updated'
            }
          })
        }, error => {
          console.error(error);
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
