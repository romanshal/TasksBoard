import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainComponent } from './main/main.component';
import { SigninComponent } from './auth/signin/signin.component';
import { NotFoundComponent } from './error/not-found/not-found.component';
import { BadRequestComponent } from './error/bad-request/bad-request.component';
import { ForbiddenComponent } from './error/forbidden/forbidden.component';
import { AuthGuard } from './auth/guard/auth.guard';
import { OAuthModule } from 'angular-oauth2-oidc';
import { BoardComponent } from './board/board.component';
import { UnauthorizedComponent } from './error/unauthorized/unauthorized.component';
import { BoardsListComponent } from './boards-list/boards-list.component';
import { SignupComponent } from './auth/signup/signup.component';
import { ProfileComponent } from './profile/profile.component';
import { InternalServerErrorComponent } from './error/internal-server-error/internal-server-error.component';
import { NotificationComponent } from './notification/notification.component';
import { ExternalCallbackComponent } from './auth/external-callback/external-callback.component';
import { ForgotPasswordComponent } from './auth/forgot-password/forgot-password.component';
import { TwoFactorComponent } from './auth/two-factor/two-factor.component';
import { ResetPasswordComponent } from './auth/reset-password/reset-password.component';

const routes: Routes = [
  { path: '', component: MainComponent },

  { path: 'signin', component: SigninComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'external-callback', component: ExternalCallbackComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'two-factor', component: TwoFactorComponent },

  { path: 'board/:boardid', component: BoardComponent, canActivate: [AuthGuard] },
  { path: 'boards', component: BoardsListComponent, canActivate: [AuthGuard] },
  { path: 'boards/public', component: BoardsListComponent, canActivate: [AuthGuard] },

  { path: 'profile/:userid', component: ProfileComponent, canActivate: [AuthGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },

  { path: 'notification', component: NotificationComponent, canActivate: [AuthGuard] },

  { path: 'bad-request', component: BadRequestComponent },
  { path: 'forbidden', component: ForbiddenComponent },
  { path: 'unauthorized', component: UnauthorizedComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'internal-server-error', component: InternalServerErrorComponent },

  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    OAuthModule.forRoot()
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
