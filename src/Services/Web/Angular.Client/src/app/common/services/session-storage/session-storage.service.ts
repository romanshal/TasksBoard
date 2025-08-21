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
    public currentDeviceId = 'device_id';

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

    getDeviceId(){
        return sessionStorage.getItem(this.currentDeviceId);
    }

    setDeviceId(value: string){
        sessionStorage.setItem(this.currentDeviceId, value);
    }

    logout() {
        sessionStorage.clear();
    }
}
