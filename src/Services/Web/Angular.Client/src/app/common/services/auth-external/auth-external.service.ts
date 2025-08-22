import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthExternalService {

  constructor(
    private authService: AuthService,
    private http: HttpClient,
  ) { }

  signin(){
    const url = '/api/external-login';
  }
}
