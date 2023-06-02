import { Injectable } from '@angular/core';
import { WebApi } from '../shared/web-api';
import { SignupRequest } from './models/signup-request.model';
import { REFERENCE_PREFIX } from '@angular/compiler/src/render3/view/util';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';

export const SIGNUP_API_URL = 'user/register';

@Injectable({
  providedIn: 'root'
})
export class SignupService {

  firstName: string = '';
  lastName: string = '';
  phoneNumber: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  message: string = '';
  showLoading: boolean = false;

  constructor(
    private webApi: WebApi,
    private router: Router
  ) { }

  register() {
    this.message = '';
    if (this.password !== this.confirmPassword) {
      this.message = 'Password not match!';
      return;
    }

    const request: SignupRequest = {
      userName: this.firstName + ' ' + this.lastName,
      email: this.email,
      password: this.password,
      phoneNumber: this.phoneNumber
    };
    this.showLoading = true;
    this.webApi.postData<number>(request, SIGNUP_API_URL)
    .pipe(
      // tap((data: number) => {
      //   this.storeToken(data.accessToken, 'temp');
      //   this.authenticatedSubject.next(this.checkAuthenticated());
      //   this.router.navigateByUrl('main');
      // })
    ).subscribe((data: number) => {
      if (data !== 0) {
        this.showLoading = false;
        this.message = "Signup success! Let's login";
        setTimeout(()=> {
          this.router.navigateByUrl('login');
        }, 3000);
      }
    }, error => {
      this.message = error.error;
      this.showLoading = false;
    });
    
  }
}
