import * as http from "@angular/common/http";
import { Observable, tap } from "rxjs";
import { NotificationService } from "../_services/notification.service";
import { UserService } from "../user/_services/user-service.service";
import { Injectable } from "@angular/core";
import { ApiResponse } from "../_models/api-response";
import { SpinnerService } from "../_services/sipnner.service";

@Injectable()
export class ErrorInterceptor implements http.HttpInterceptor {
    constructor(
        private userService: UserService,
        private loaderService: SpinnerService,
        private notificationService: NotificationService,
    ) { }

    intercept(request: http.HttpRequest<any>, next: http.HttpHandler): Observable<http.HttpEvent<any>> {

        //start spinner

        return this.handler(next, request);
    }

    handler(next: http.HttpHandler, request: http.HttpRequest<any>) {
        return next.handle(request).pipe(tap((event) => {
            if (event instanceof http.HttpResponse) {
                //stop spinner
                this.loaderService.requestEnded()
            }
        }, (err: http.HttpErrorResponse) => {
            if ([400].includes(err.status)) {
                this.notificationService.ShowError(err.message);
            }
            if ([401, 403].includes(err.status) && this.userService.userValue) {
                // auto logout if 401 or 403 response returned from api
                this.userService.logout();

                //stop spinner
                this.loaderService.requestEnded()
                this.notificationService.ShowError("Some Error Occurred. Please Login again.")
            }

            //stop spinner
            this.loaderService.requestEnded()

            const error = err.error?.message || err.statusText;
            console.error(err);
            this.notificationService.ShowError(error)
        }
        ))

    }
}

@Injectable()
export class ResponseInterceptor implements http.HttpInterceptor {
    constructor(
        private notificationService: NotificationService,
    ) { }

    intercept(request: http.HttpRequest<any>, next: http.HttpHandler): Observable<http.HttpEvent<ApiResponse<any>>> {
        return next.handle(request).pipe(
            tap((event: http.HttpEvent<ApiResponse<any>>) => {
                if (event instanceof http.HttpResponse) {
                    // change the response body here
                    // debugger;
                    if (event.body?.failed) this.notificationService.ShowError(event.body.messages);
                    var eve = event.clone({
                        body: event.body?.data
                    });

                    return eve;
                }

                return event;
            })
        );
    }
}
