import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ApiResponse } from '../_models/api-response';
import Vw_ListTypeItemsResponse from '../_responses/vw-list-type-items-response';

@Injectable({
  providedIn: 'root'
})
export class MasterDataService {
  constructor(private http: HttpClient) { }

  GetListItems(listCode: string) {
    return this.http.get<ApiResponse<Vw_ListTypeItemsResponse[]>>(`${environment.apiUrl}/MasterData/GetByListCode/${listCode}`)
      .pipe(map(x => x.failed ? [] : x.data));
  }

}
