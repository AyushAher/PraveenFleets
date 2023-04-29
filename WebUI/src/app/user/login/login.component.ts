import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../_services/user-service.service';
import { NotificationService } from 'src/app/_services/notification.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ["./login-component.css"]
})
export class LoginComponent {

  LoginForm: FormGroup;

  constructor(
    _FormBuilder: FormBuilder,
    private userService: UserService,
    private notificationService: NotificationService
  ) {
    this.LoginForm = _FormBuilder.group({
      email: ["", [Validators.required]],
      password: ["", [Validators.required]],
    });

  }


  OnSubmit() {
    this.LoginForm.markAllAsTouched();
    if (this.LoginForm.invalid) return;

    var formData = this.LoginForm.value;

    this.userService.Login(formData)
      .subscribe((x) => {
        if (x) this.notificationService.ShowSuccess("Login Success")
      });

  }

}
