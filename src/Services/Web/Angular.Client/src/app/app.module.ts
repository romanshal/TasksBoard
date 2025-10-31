import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { MatDialogModule } from '@angular/material/dialog';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './main/main.component';
import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SigninComponent } from './auth/signin/signin.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AuthGuard } from './auth/guard/auth.guard';
import { AuthRequestInterceptor } from './common/interceptors/auth-request.interceptor';
import { NotFoundComponent } from './error/not-found/not-found.component';
import { ForbiddenComponent } from './error/forbidden/forbidden.component';
import { BadRequestComponent } from './error/bad-request/bad-request.component';
import { BoardComponent } from './board/board.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BoardNoticeModal } from './common/modals/board-notice/board-notice.modal';
import { ProfileMenuModal } from './common/modals/profile-menu/profile-menu.modal';
import { PaginatorComponent } from './common/paginator/paginator.component';
import { UnauthorizedComponent } from './error/unauthorized/unauthorized.component';
import { BoardsListComponent } from './boards-list/boards-list.component';
import { SignupComponent } from './auth/signup/signup.component';
import { ChatComponent } from './chat/chat.component';
import { BoardMembersModal } from './common/modals/board-members/board-members.modal';
import { BoardInfoModal } from './common/modals/board-info/board-info.modal';
import { NgxSpinnerModule } from 'ngx-spinner';
import { BoardMemberPermissionsModal } from './common/modals/board-member-permissions/board-member-permissions.modal';
import { InviteMemberModal } from './common/modals/invite-member/invite-member.modal';
import { AutoResizeTextareaDirective } from './common/directives/auto-resize-textarea.directive';
import { DeleteConfirmationModal } from './common/modals/delete-confirmation/delete-confirmation.modal';
import { BoardMemberRequestModal } from './common/modals/board-member-request/board-member-request.modal';
import { InternalServerErrorComponent } from './error/internal-server-error/internal-server-error.component';
import { QRCodeComponent } from 'angularx-qrcode';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { ProfileComponent } from './profile/profile.component';
import { OperationResultMessageModal } from './common/modals/operation-result-message/operation-result-message.modal';
import { ProfileAvatarModal } from './common/modals/profile-avatar/profile-avatar.modal';
import { NotificationMenuModal } from './common/modals/notification-menu/notification-menu.modal';
import { NotificationComponent } from './notification/notification.component';
import { PaginatorNewComponent } from './common/paginator-new/paginator-new.component';
import { PickerComponent } from '@ctrl/ngx-emoji-mart';
import { ExternalCallbackComponent } from './auth/external-callback/external-callback.component';
import { TwoFactorComponent } from './auth/two-factor/two-factor.component';
import { ForgotPasswordComponent } from './auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './auth/reset-password/reset-password.component';

@NgModule({
  declarations: [
    AppComponent,
    FooterComponent,
    HeaderComponent,
    SigninComponent,
    SignupComponent,
    NotFoundComponent,
    MainComponent,
    NotFoundComponent,
    ForbiddenComponent,
    BadRequestComponent,
    BoardComponent,
    BoardNoticeModal,
    ProfileMenuModal,
    PaginatorComponent,
    UnauthorizedComponent,
    BoardsListComponent,
    ChatComponent,
    BoardMembersModal,
    BoardInfoModal,
    BoardMemberPermissionsModal,
    InviteMemberModal,
    AutoResizeTextareaDirective,
    DeleteConfirmationModal,
    BoardMemberRequestModal,
    InternalServerErrorComponent,
    ProfileComponent,
    OperationResultMessageModal,
    ProfileAvatarModal,
    NotificationMenuModal,
    NotificationComponent,
    PaginatorNewComponent,
    ExternalCallbackComponent,
    TwoFactorComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    HttpClientModule,
    NgxSpinnerModule,
    QRCodeComponent,
    ClipboardModule,
    PickerComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    AuthGuard,
    { provide: HTTP_INTERCEPTORS, useClass: AuthRequestInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
