import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { ApiResponse } from "../_models/api-response";
import EnumResponse from "../_responses/enum-response";
import { first, map } from "rxjs";

@Injectable({ providedIn: "any" })
export default class EnumService {
    constructor(private http: HttpClient) { }

    GetEnumList(enumName: string) {
        return this.http.get<ApiResponse<EnumResponse[]>>(`${environment.apiUrl}/Enum/${enumName}`)
            .pipe(map(x => x.failed ? [] : x.data));
    }

}