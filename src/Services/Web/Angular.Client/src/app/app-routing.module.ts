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

const routes: Routes = [
  { path: '', component: MainComponent },
  { path: 'signin', component: SigninComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'board/:boardid', component: BoardComponent, canActivate: [AuthGuard] },
  { path: 'boards', component: BoardsListComponent, canActivate: [AuthGuard] },
  { path: 'boards/public', component: BoardsListComponent, canActivate: [AuthGuard] },

  { path: 'bad-request', component: BadRequestComponent },
  { path: 'forbidden', component: ForbiddenComponent },
  { path: 'unauthorized', component: UnauthorizedComponent },

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
