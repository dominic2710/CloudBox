import { Component, OnInit } from '@angular/core';
import { LoginService } from './login.service';
import { LoginRequest } from './models/login-request.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  email: string = '';
  password: string = '';

  constructor(
    public _service: LoginService
  ) { }

  ngOnInit(): void {
  }

  onLogin(): void {
    const request: LoginRequest = {
      email: this.email,
      password: this.password
    }
    this._service.login(request);
  }
}
