import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Guid } from 'guid-typescript';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { ApiResponse } from 'src/app/_models/api-response';
import ConfirmEmailRequest from 'src/app/_requests/confirm-email';
import { LoginRequest } from 'src/app/_requests/login-request';
import { RegisterUserRequest } from 'src/app/_requests/register-request';
import { LoginResponse } from 'src/app/_responses/login-response';
import { UserResponse } from 'src/app/_responses/user-response';
import { NotificationService } from 'src/app/_services/notification.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  userSubject: BehaviorSubject<UserResponse | null>;
  private user: Observable<UserResponse | null>;
  returnUrl: any;

  constructor(
    private http: HttpClient,
    private router: Router,
    private activeRoute: ActivatedRoute,
    private notificationService: NotificationService
  ) {
    var storedUser = localStorage.getItem('user') || JSON.stringify("");
    this.userSubject = new BehaviorSubject<UserResponse | null>(JSON.parse(storedUser));
    this.user = this.userSubject.asObservable();
    this.returnUrl = this.activeRoute.snapshot.queryParams.returnUrl
  }

  Login(formData: LoginRequest) {
    return this.http.post<ApiResponse<LoginResponse>>(`${environment.apiUrl}/User/Authenticate`, formData)
      .pipe(
        map(x => {
          if (x.failed) {
            this.notificationService.ShowError(x.messages)
            return null
          }

          if (!x.data) {
            this.notificationService.ShowError(x.messages)
            return null;
          }

          this.GetUserById(x.data.userId)
            .subscribe(userResponse => {
              if (!userResponse) return null;
              userResponse.token = x.data;
              this.userSubject.next(userResponse);
              localStorage.setItem('user', JSON.stringify(userResponse));
              this.router.navigate([this.returnUrl || "/"]);
              return;
            })

          return x.data;
        })
      );
  }

  Register(formData: RegisterUserRequest) {
    return this.http.post<ApiResponse<string>>(`${environment.apiUrl}/User/Register`, formData)
      .pipe(
        map(x => {
          if (x.failed) return null
          if (!x.data) return;

          var data = x.data;
          var userId = data?.split(":")[1];

          if (!userId || !Guid.isGuid(userId)) return null;

          var loginRequest: LoginRequest = {
            email: formData.email,
            password: formData.password
          }

          this.Login(loginRequest)
            .subscribe();

          return x.data;
        })
      );
    ;
  }

  ConfirmEmail(formData: ConfirmEmailRequest) {
    return this.http.post<ApiResponse<any>>(`${environment.apiUrl}/User/ConfirmEmail`, formData)
      .pipe(map(x => x.messages));
  }

  logout() {
    localStorage.clear();
    this.userSubject.next(null);
    this.router.navigate(['/user', 'login']);
  }


  GetUserById(id: string) {
    return this.http.get<ApiResponse<UserResponse>>(`${environment.apiUrl}/User/Get/${id}`)
      .pipe(map(x => x.failed ? null : x.data))
  }

  public get userValue(): UserResponse | null {
    return this.userSubject.value;
  }

}

