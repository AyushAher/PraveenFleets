import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from "rxjs";
import { ApiResponse } from "src/app/_models/api-response";
import { RegisterUserRequest } from "src/app/_requests/register-request";
import { environment } from "src/environments/environment";

@Injectable({ providedIn: "any" })
export default class OrganizationEmployeeService {

    constructor(private http: HttpClient) { }

    RegisterEmployee(FormData: RegisterUserRequest) {
        return this.http.post<ApiResponse<any>>(`${environment.apiUrl}/OrganizationEmployee/RegisterEmployee`, FormData)
            .pipe(
                map((x: ApiResponse<any>) => {
                    if (x.failed) return null
                    if (!x.data) return null;
                    return x.data;
                })
            )
    }

}