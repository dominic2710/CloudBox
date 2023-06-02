import { Component, OnInit } from '@angular/core';
import { SignupService } from './signup.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  constructor(
    public service: SignupService
  ) { }

  ngOnInit(): void {
  }

  onClickSignUp() {
    this.service.register();
  }
}
