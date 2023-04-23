import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { _ } from "ag-grid-community";
import { map } from "rxjs";
import { ApiResponse } from "src/app/_models/api-response";
import CreateOrganizationRolesRequest from "src/app/_requests/create-organization-roles-request";
import OrganizationRoleResponse from "src/app/_responses/organization-role-response";
import { environment } from "src/environments/environment";

@Injectable({ providedIn: "any" })
export default class OrganizationRoleService {
    public _object: CreateOrganizationRolesRequest;

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

    SaveRole() {
        return this.http.post<ApiResponse<boolean>>(`${environment.apiUrl}/OrganizationRole/CreateOrganizationRole`, this._object)
            .pipe(
                map(x => {
                    if (x.failed) return false;
                    if (!x.data) return false;
                    return x.data;
                })
            )
    }

}