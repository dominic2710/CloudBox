import { Component, ElementRef, OnInit } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {

  showDropdownSidebar = false;
  
  constructor(private elementRef: ElementRef) {}

  ngOnInit() {
    document.addEventListener('click', this.onDocumentClick);
  }

  ngOnDestroy() {
    document.removeEventListener('click', this.onDocumentClick);
  }

  showSideMenu(): void {
    this.showDropdownSidebar = true;
  }

  onDocumentClick = (event: MouseEvent) => {
    const clickedInside = this.elementRef.nativeElement.contains(event.target);
    if (!clickedInside) {
      this.showDropdownSidebar = false;
    }
  };
}
