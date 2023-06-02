import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, CanLoad, Route, Router, RouterStateSnapshot, UrlSegment, UrlTree } from "@angular/router";
import { LoginService } from "../login/login.service";
import { Observable, of } from "rxjs";
import { Location } from '@angular/common';

@Injectable({
    providedIn: 'root'
  })
  export class AuthGuard implements CanActivate, CanActivateChild, CanLoad {
    constructor(
      private router: Router,
      private location: Location,
      private loginService: LoginService,
      //private refreshTokenService: RefreshTokenService,
      //@Inject(ERROR_UNAUTHORIZED_URL) private unauthorizedUrl: string
    ) { }
  
    /**
     * 認証のチェックを行う
     *
     * 認証が許可されていない場合、一度リフレッシュトークンを用いて、認証トークンをリフレッシュする。
     * リフレッシュトークンの有効期限も切れている場合、LOGIN_URLへリダイレクトする。
     * リダイレクトの際にクエリパラメータのreturnUrlにアクセスしたURLをセットする。
     *
     * @param next ActivatedRouteSnapshot
     * @param state RouterStateSnapshot
     */
    canActivate(
      next: ActivatedRouteSnapshot,
      state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
        if (!this.loginService.accessToken || !this.loginService.refreshToken) {
          this.router.navigate(['login'], { queryParams: { returnUrl: state.url } });
          return of(false);
        }

        if (!this.loginService.checkAuthenticated()) {
            this.router.navigate(['login'], { queryParams: { returnUrl: state.url } });
            return of(false);
        }

  
        // if (this.refreshTokenService.isNeedRefreshToken()) {
        //   return this.refreshTokenService.refreshAccessToken()
        //       .pipe(
        //           map(_ => { return true; }),
        //           catchError(_ => {
        //               this.authService.clearTokenAndUserCache();
        //               this.router.navigate([this.unauthorizedUrl], { queryParams: { returnUrl: state.url } });
        //               return of(false);
        //           })
        //       );
        // }
  
        // if (this.refreshTokenService.hasToWaitForRefresh()) {
        //   return this.refreshTokenService.waitForAccessTokenRefresh()
        //       .pipe(
        //         map(_ => { return true; }),
        //         catchError(_ => {
        //             this.router.navigate([this.unauthorizedUrl], { queryParams: { returnUrl: state.url } });
        //             return of(false);
        //         })
        //       );
        // }
  
        return of(true);
    }
  
    canActivateChild(
      next: ActivatedRouteSnapshot,
      state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      return this.canActivate(next, state);
    }

    canLoad(
        route: Route,
        segments: UrlSegment[]): Observable<boolean> | Promise<boolean> | boolean {
    
        if (!this.loginService.accessToken || !this.loginService.refreshToken) {
          this.router.navigate(['login'], { queryParams: { returnUrl: this.location.path() } });
          return of(false);
        }

        if (!this.loginService.checkAuthenticated()) {
            this.router.navigate(['login'], { queryParams: { returnUrl: this.location.path() } });
            return of(false);
        }
    
        // if (this.refreshTokenService.isNeedRefreshToken()) {
        //   return this.refreshTokenService.refreshAccessToken()
        //       .pipe(
        //           map(_ => { return true; }),
        //           catchError(_ => {
        //               this.authService.clearTokenAndUserCache();
        //               this.router.navigate([this.unauthorizedUrl], { queryParams: { returnUrl: this.location.path() } });
        //               return of(false);
        //           })
        //       );
        // }
    
        // if (this.refreshTokenService.hasToWaitForRefresh()) {
        //   return this.refreshTokenService.waitForAccessTokenRefresh()
        //       .pipe(
        //         map(_ => { return true; }),
        //         catchError(_ => {
        //             this.router.navigate([this.unauthorizedUrl], { queryParams: { returnUrl: this.location.path() } });
        //             return of(false);
        //         })
        //       );
        // }
    
        return of(true);
      }
}