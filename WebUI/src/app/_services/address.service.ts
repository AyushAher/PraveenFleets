import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ApiResponse } from '../_models/api-response';
import { AddressResponse } from '../_responses/address-response';

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  constructor(private http: HttpClient) { }

  GetAddressByParentId(parentId: string) {
    return this.http.get<ApiResponse<AddressResponse>>(`${environment.apiUrl}/Address/GetAddressByParentId/${parentId}`)
      .pipe(map(x => x.failed ? new AddressResponse() : x.data));
  }

  GetAddressByCurrentUserParentId() {
    return this.http.get<ApiResponse<AddressResponse>>(`${environment.apiUrl}/Address/GetAddressByCurrentUserParentId`)
      .pipe(map(x => x.failed ? new AddressResponse() : x.data));
  }
  
  GetAddressByCurrentUserId() {
    return this.http.get<ApiResponse<AddressResponse>>(`${environment.apiUrl}/Address/GetAddressByCurrentUserId`)
      .pipe(map(x => x.failed ? new AddressResponse() : x.data));
  }
}
