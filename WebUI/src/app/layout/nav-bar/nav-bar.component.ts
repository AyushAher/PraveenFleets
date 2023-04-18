import { Component, OnInit } from '@angular/core';
import { UserResponse } from 'src/app/_responses/user-response';
import { UserService } from 'src/app/user/_services/user-service.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent {

  isExpanded = false;
  User: UserResponse | null;

  constructor(private userService: UserService) {
    userService.userSubject.subscribe((data => this.User = data));
  }

  Logout() {
    this.userService.logout()
  }
  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

}
