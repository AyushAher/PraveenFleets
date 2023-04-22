import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from "rxjs";
import { ApiResponse } from "src/app/_models/api-response";
import OrganizationRoleResponse from "src/app/_responses/organization-role-response";
import { environment } from "src/environments/environment";

@Injectable({ providedIn: "any" })
export default class OrganizationRoleService {

    constructor(private http: HttpClient) { }

    GetAllByOrganization(organizationId: string) {
        return this.http.get<ApiResponse<OrganizationRoleResponse[]>>(`${environment.apiUrl}/OrganizationRole/GetAllOrganizationRoles/${organizationId}`)
            .pipe(
                map((x: ApiResponse<OrganizationRoleResponse[]>) => {
                    if (x.failed) return []
                    if (!x.data) return [];
                    return x.data;
                })
            )
    }

}