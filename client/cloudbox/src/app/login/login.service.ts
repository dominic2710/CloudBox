import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { API_URL } from '../const/injection-tokens';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { LoginRequest } from './models/login-request.model';
import { LoginResponse } from './models/login-response.model';
import { tap, catchError, take, finalize } from 'rxjs/operators';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from './models/user.model';
import { WebApi } from '../shared/web-api';

export const LOCAL_STORAGE_ACCESS_TOKEN = 'accessToken';
export const LOCAL_STORAGE_REFRESH_TOKEN = 'refreshToken';
export const LOGIN_API_URL = 'user/login';
export const LOGOUT_API_URL = 'user/logout';
export const FETCHME_API_URL = 'user/me';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  private loginUserSubject: BehaviorSubject<User>;
  private authenticatedSubject: BehaviorSubject<boolean>;

  message: string = '';

  get loginUser(): Observable<User> {
    if (this.checkAuthenticated()
        && !this.loginUserSubject.value.id) {
      this.fetchMe();
    }
    return this.loginUserSubject.asObservable();
  }

  get accessToken(): string {
    const token = localStorage.getItem(LOCAL_STORAGE_ACCESS_TOKEN);
    return token ? token : '';
  }

  get refreshToken(): string {
    const token = localStorage.getItem(LOCAL_STORAGE_REFRESH_TOKEN);
    return token ? token : '';
  }

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(API_URL) private apiUrl: string,
    private webApi: WebApi
  ) {
    this.loginUserSubject = new BehaviorSubject<User>({} as User)
    this.authenticatedSubject = new BehaviorSubject(this.checkAuthenticated());
  }

  public login(body: LoginRequest): void {
    this.message = '';
    this.webApi.postData<LoginResponse>(body, LOGIN_API_URL)
    .pipe(
      tap((data: LoginResponse) => {
        this.storeToken(data.accessToken, 'temp');
        this.authenticatedSubject.next(this.checkAuthenticated());
        this.router.navigateByUrl('main');
      })
    ).subscribe((data: LoginResponse) => {
      const user: User = { ...data};
      this.loginUserSubject.next(user);
    }, error => {
      this.message = error.error;
    });
  }

  public logout(): void {
    this.webApi.getData<number>(LOGOUT_API_URL)
      .pipe(finalize(() => {
        this.clearTokenAndUserCache();
        this.router.navigateByUrl('login');
      }))
      .subscribe();
  }

  private storeToken(accessToken: string, refreshToken: string): void {
    localStorage.setItem(LOCAL_STORAGE_ACCESS_TOKEN, accessToken);
    localStorage.setItem(LOCAL_STORAGE_REFRESH_TOKEN, refreshToken);
  }

  private removeToken(): void {
    localStorage.removeItem(LOCAL_STORAGE_ACCESS_TOKEN);
    localStorage.removeItem(LOCAL_STORAGE_REFRESH_TOKEN);
  }

  public checkAuthenticated(): boolean {
    if (this.accessToken) {
      const payload = this.parseJwt(this.accessToken);
      const expiresAt = payload.exp;
      return expiresAt > Date.now() / 1000;
    } else {
      return false;
    }
  }

  public parseJwt(token: string): any {
    const base64UrlPayload = token.split('.')[1];
    const base64Payload = base64UrlPayload.replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(decodeURIComponent(escape(window.atob(base64Payload))));
  }

  public clearTokenAndUserCache() {
    this.removeToken();
    this.loginUserSubject.next({} as User);
    this.authenticatedSubject.next(this.checkAuthenticated());
  }

  public fetchMe() {
    this.message = '';
    this.webApi.getData<LoginResponse>(FETCHME_API_URL)
    .pipe(
      tap((data: LoginResponse) => {
        this.authenticatedSubject.next(this.checkAuthenticated());
        this.router.navigateByUrl('main');
      })
    ).subscribe((data: LoginResponse) => {
      const user: User = { ...data};
      this.loginUserSubject.next(user);
    }, error => {
      this.message = error.error;
    });
  }
}

