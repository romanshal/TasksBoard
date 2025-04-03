import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { MatDialogModule } from '@angular/material/dialog';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './main/main.component';
import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SigninComponent } from './auth/signin/signin.component';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { AuthGuard } from './auth/guard/auth.guard';
import { AuthRequestInterceptor } from './common/interceptors/auth-request.interceptor';
import { NotFoundComponent } from './error/not-found/not-found.component';
import { ResponseInterceptor } from './common/interceptors/response.interceptor';
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
    HttpClientModule
  ],
  providers: [
    AuthGuard,
    { provide: HTTP_INTERCEPTORS, useClass: AuthRequestInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ResponseInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
