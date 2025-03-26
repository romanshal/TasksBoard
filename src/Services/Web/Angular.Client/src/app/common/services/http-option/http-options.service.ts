import { HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class HttpOptionService {
    public getOptions(key: any, value: any) {
        return new HttpParams().append(key, value);
    }

    public getSeveralOptions(key1: any, value1: any, key2: any, value2: any) {
        return new HttpParams().append(key1, value1).append(key2, value2);
    }

    public getPaginationOptions(pageIndex: number, pageSize: number) {
        return new HttpParams().append('pageindex', pageIndex).append('pagesize', pageSize);
    }
}