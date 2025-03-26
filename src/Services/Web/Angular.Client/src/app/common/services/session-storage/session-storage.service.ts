import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class SessionStorageService {
    public accessTokenKey = 'access_token';
    public refreshTokenKey = 'refresh_token';
    public userIdKey = 'user_id'

    getItem(key:string){
        return sessionStorage.getItem(key);
    }

    setItem(key: string, value: string){
        sessionStorage.setItem(key, value);
    }

    getAccessToken(){
        return sessionStorage.getItem(this.accessTokenKey);
    }

    setAccessToken(value: string){
        sessionStorage.setItem(this.accessTokenKey, value);
    }

    getRefreshToken(){
        return sessionStorage.getItem(this.refreshTokenKey);
    }

    setRefreshToken(value: string){
        sessionStorage.setItem(this.refreshTokenKey, value);
    }

    logout(){
        sessionStorage.removeItem(this.accessTokenKey);
        sessionStorage.removeItem(this.refreshTokenKey);
    }
}
