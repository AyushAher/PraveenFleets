import { Component } from '@angular/core';
import { UserService } from '../user/_services/user-service.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  constructor(userService: UserService) {
    console.log(userService.userValue);

  }
}
