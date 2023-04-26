import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from "rxjs";
import { ApiResponse } from "src/app/_models/api-response";
import { RegisterUserRequest } from "src/app/_requests/register-request";
import OrganizationEmployeeResponse from "src/app/_responses/organization-employee-response";
import { environment } from "src/environments/environment";

@Injectable({ providedIn: "any" })
export default class OrganizationEmployeeService {

    constructor(private http: HttpClient) { }

    RegisterEmployee(FormData: RegisterUserRequest) {
        return this.http.post<ApiResponse<OrganizationEmployeeResponse>>(`${environment.apiUrl}/OrganizationEmployee/RegisterEmployee`, FormData)
            .pipe(
                map((x: ApiResponse<OrganizationEmployeeResponse>) => {
                    if (x.failed) return null
                    if (!x.data) return null;
                    return x.data;
                })
            )
    }

    GetAllOrganizationEmployees() {
        return this.http.get<ApiResponse<OrganizationEmployeeResponse[]>>(`${environment.apiUrl}/OrganizationEmployee/GetAllOrganizationEmployees`)
            .pipe(
                map((x: ApiResponse<OrganizationEmployeeResponse[]>) => {
                    if (x.failed) return [];
                    if (!x.data) return [];
                    return x.data;
                })
            )
    }

}