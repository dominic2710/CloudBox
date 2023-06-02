import { HttpClient, HttpHeaders } from "@angular/common/http";
import { API_URL } from "../const/injection-tokens";
import { Inject, Injectable } from "@angular/core";

export const LOCAL_STORAGE_ACCESS_TOKEN = 'accessToken';
export const LOCAL_STORAGE_REFRESH_TOKEN = 'refreshToken';

@Injectable({
    providedIn: 'root'
  })
export class WebApi {
    
    constructor(
        private http: HttpClient,
        @Inject(API_URL) private apiUrl: string,
    ) {
    }

    private rootEndPointApi = () => {
        let endP = this.apiUrl || '/';
        let returnString = '';
        if (endP == '/') {
          returnString = endP;
        } else {
          returnString = endP + '/';
        }
        return returnString;
    }

    get accessToken(): string {
        const token = localStorage.getItem(LOCAL_STORAGE_ACCESS_TOKEN);
        return token ? token : '';
      }
    
      get refreshToken(): string {
        const token = localStorage.getItem(LOCAL_STORAGE_REFRESH_TOKEN);
        return token ? token : '';
      }
    
    private headersContent(): HttpHeaders {
        return new HttpHeaders({
          'Content-Type': 'application/json; charset=utf-8',
          'Authorization': this.accessToken
        });
    }

    public postData<T>(param: any, url: string) {
        return this.http
          .post<T>(this.rootEndPointApi() + url, JSON.stringify(param), { headers: this.headersContent() });
    }

    public getData<T>(url: string) {
        return this.http
          .get<T>(this.rootEndPointApi() + url, { headers: this.headersContent() });
      }
}