import { Injectable } from "@angular/core";
import { UserInfoModel } from "../../models/user/user-info.model";

@Injectable({
    providedIn: 'root'
})
export class SessionStorageService {
    public accessTokenKey = 'access_token';
    private userKey = 'user';
    public currentDeviceId = 'device_id';

    getItem(key: string): string | null {
        return sessionStorage.getItem(key);
    }

    setItem(key: string, value: string) {
        sessionStorage.setItem(key, value);
    }

    getUserInfo(): UserInfoModel | null {
        return JSON.parse(sessionStorage.getItem(this.userKey)!) as UserInfoModel;
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

    getDeviceId() {
        return sessionStorage.getItem(this.currentDeviceId);
    }

    setDeviceId(value: string) {
        sessionStorage.setItem(this.currentDeviceId, value);
    }

    logout() {
        sessionStorage.clear();
    }
}
