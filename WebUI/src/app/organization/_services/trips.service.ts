import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { ApiResponse } from 'src/app/_models/api-response';
import TripsRequest from 'src/app/_requests/trips-request';
import TripsResponse from 'src/app/_responses/trips-response';
import { NotificationService } from 'src/app/_services/notification.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TripsService {
  constructor(
    private http: HttpClient,
    private notificationService: NotificationService
  ) { }

  SaveTripDraft(FormData: TripsRequest) {
    return this.http.post<ApiResponse<TripsResponse>>(`${environment.apiUrl}/Trips/SaveAsDraft`, FormData)
      .pipe(
        map((x: ApiResponse<TripsResponse>) => {
          if (x.failed) return null
          if (!x.data) {
            this.notificationService.ShowError("Unable to Save Trip Draft")
            return null
          };
          this.notificationService.ShowSuccess("Draft Saved Successfully!")
          return x.data;
        })
      )
  }

  GetTripsByCurrentOrg() {
    return this.http.get<ApiResponse<TripsResponse[]>>(`${environment.apiUrl}/Trips/GetTripsByCurrentOrg`)
      .pipe(
        map((x: ApiResponse<TripsResponse[]>) => {
          if (x.failed) return []
          if (!x.data) {
            this.notificationService.ShowError("Unable to get Trip Data")
            return []
          };
          return x.data;
        })
      )
  }
}
