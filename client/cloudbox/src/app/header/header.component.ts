import { Component, ElementRef, OnInit } from '@angular/core';
import { LoginService } from '../login/login.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  isShowDropdownSearch = false;
  isShowUserInfoModal = false;
  isShowHelpMenu = false;
  isShowSettingMenu = false;

  loginUser$ = this.loginService.loginUser;

  dropdownIds: string[] = ['dropdownSearch', 'dropdownHelp', 'dropdownSetting', 'dropdownUser'];

  constructor(
    private elementRef: ElementRef,
    private loginService: LoginService) { }

  ngOnInit() {
    document.addEventListener('click', this.onDocumentClick);
  }

  ngOnDestroy() {
    document.removeEventListener('click', this.onDocumentClick);
  }

  showSearchMenu(): void {
    this.hideAllMenu();
    this.isShowDropdownSearch = true;
  }

  showUserInfo(): void {
    this.hideAllMenu();
    this.isShowUserInfoModal =true;
  }

  showHelpMenu(): void {
    this.hideAllMenu();
    this.isShowHelpMenu = true;
  }

  showSettingMenu(): void {
    this.hideAllMenu();
    this.isShowSettingMenu = true;
  }

  hideAllMenu(): void {
    this.isShowDropdownSearch = false;
    this.isShowUserInfoModal = false;
    this.isShowHelpMenu = false;
    this.isShowSettingMenu = false;
  }

  onDocumentClick = (event: MouseEvent) => {
    const target = event.target as HTMLElement;
    const id = target.id;

    if (this.dropdownIds.filter(x=>x === id).length === 0)
      this.hideAllMenu();
  }

  onClickLogout(): void {
    this.loginService.logout();
  }
}
