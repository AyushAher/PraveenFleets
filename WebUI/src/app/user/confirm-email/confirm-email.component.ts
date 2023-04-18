import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../_services/user-service.service';
import ConfirmEmailRequest from 'src/app/_requests/confirm-email';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html'
})
export class ConfirmEmailComponent implements OnInit {

  ConfirmEmailRequest: ConfirmEmailRequest
  DisplayMessage: string = "Please wait while we confirm your email address. You will be redirected to login page once confirmed."
  Count: number;

  constructor(
    private activeRoute: ActivatedRoute,
    private router: Router,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    let emailId = this.activeRoute.snapshot.queryParamMap.get("useremail");
    let token = this.activeRoute.snapshot.queryParamMap.get("token");
    console.log(this.activeRoute.snapshot);

    if (emailId == null || token == null) {
      this.DisplayMessage = "Invalid URL.";
      return;
    }

    this.ConfirmEmailRequest = new ConfirmEmailRequest();
    this.ConfirmEmailRequest.email = emailId;
    this.ConfirmEmailRequest.token = token;

    this.userService.ConfirmEmail(this.ConfirmEmailRequest)
      .subscribe((data: any) => {
        this.DisplayMessage = data;
        this.Count = 10;

        var decrementCount = setInterval(() => {
          this.DisplayMessage = data + `\n Redirecting to home page in ${this.Count}`;
          this.Count -= 1;
        }, 1000);
        setTimeout(() => {
          clearInterval(decrementCount);
          this.router.navigate(["/"]);
        }, 10000);

      })

  }

}
