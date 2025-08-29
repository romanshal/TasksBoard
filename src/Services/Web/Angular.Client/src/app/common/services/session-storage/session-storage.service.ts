import { Injectable } from "@angular/core";
import { UserInfoModel } from "../../models/user/user-info.model";
import { AccessTokenPair } from "../../models/auth/auth.access-token-pair.model";

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
        const raw = sessionStorage.getItem(this.userKey);
        if (!raw) {
            return null;
        }

        try {
            return JSON.parse(raw) as UserInfoModel;
        } catch (e) {
            return null;
        }
    }

    setUserInfo(value: UserInfoModel) {
        sessionStorage.setItem(this.userKey, JSON.stringify(value));
    }

    getAccessToken(): AccessTokenPair | null {
        const raw = sessionStorage.getItem(this.accessTokenKey);
        if (!raw) {
            return null;
        }

        try {
            return JSON.parse(raw) as AccessTokenPair;
        } catch (e) {
            return null;
        }
    }

    setAccessToken(token: AccessTokenPair) {
        sessionStorage.setItem(this.accessTokenKey, JSON.stringify(token));
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
