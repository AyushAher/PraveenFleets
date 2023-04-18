import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from "rxjs";
import { ApiResponse } from "src/app/_models/api-response";
import { RegisterOrganizationRequest } from "src/app/_requests/register-request";
import OrganizationResponse from "src/app/_responses/Organization-response";
import { environment } from "src/environments/environment";

@Injectable({ providedIn: "any" })
export default class OrganizationService {

    constructor(private http: HttpClient) { }

    Save(FormData: RegisterOrganizationRequest) {
        return this.http.post<ApiResponse<OrganizationResponse>>(`${environment.apiUrl}/Organization/Register`, FormData)
            .pipe(
                map((x: ApiResponse<OrganizationResponse>) => {
                    if (x.failed) return null
                    if (!x.data) return;
                    return x.data;
                })
            )
    }

}