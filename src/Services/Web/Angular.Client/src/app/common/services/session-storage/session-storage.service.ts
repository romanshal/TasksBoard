import { Injectable } from "@angular/core";
import { UserInfoModel } from "../../models/user/user-info.model";

@Injectable({
    providedIn: 'root'
})
export class SessionStorageService {
    public accessTokenKey = 'access_token';
    public refreshTokenKey = 'refresh_token';
    public userIdKey = 'user_id';
    private userKey = 'user';

    getItem(key: string): string | null {
        return sessionStorage.getItem(key);
    }

    setItem(key: string, value: string) {
        sessionStorage.setItem(key, value);
    }

    getUserInfo(): UserInfoModel | undefined {
        if(sessionStorage.getItem(this.userKey)){
            return JSON.parse(sessionStorage.getItem(this.userKey)!) as UserInfoModel;
        }

        return undefined;
    }

    setUserInfo(value: UserInfoModel) {
        sessionStorage.setItem(this.userKey, JSON.stringify(value));
    }

    getAccessToken() {
        return sessionStorage.getItem(this.accessTokenKey);
    }

    setAccessToken(value: string) {
        sessionStorage.setItem(this.accessTokenKey, value);
    }

    getRefreshToken() {
        return sessionStorage.getItem(this.refreshTokenKey);
    }

    setRefreshToken(value: string) {
        sessionStorage.setItem(this.refreshTokenKey, value);
    }

    logout() {
        sessionStorage.removeItem(this.accessTokenKey);
        sessionStorage.removeItem(this.refreshTokenKey);
    }
}
