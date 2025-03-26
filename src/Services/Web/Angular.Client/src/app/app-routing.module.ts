import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainComponent } from './main/main.component';
import { LoginComponent } from './auth/login/login.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { BadRequestComponent } from './bad-request/bad-request.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { AuthGuard } from './auth/guard/auth.guard';
import { OAuthModule } from 'angular-oauth2-oidc';
import { BoardComponent } from './board/board.component';

const routes: Routes = [
  { path: '', component: MainComponent },
  { path: 'login', component: LoginComponent },
  { path: 'board/:boardid', component: BoardComponent, canActivate: [AuthGuard] },

  { path: 'bad-request', component: BadRequestComponent },
  { path: 'forbidden', component: ForbiddenComponent },
  { path: 'unauthorized', component: BadRequestComponent },

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
