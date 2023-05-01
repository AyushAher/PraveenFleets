import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TripsRoutingModule } from './trips-routing.module';
import { ScheduleTripComponent } from './schedule-trip/schedule-trip.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ResponseInterceptor, ErrorInterceptor } from '../_helpers/interceptor';
import { JwtInterceptor } from '../_helpers/jwtinterceptor';

import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { SharedModule } from '../shared/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@NgModule({
  declarations: [
    ScheduleTripComponent
  ],
  imports: [
    CommonModule,
    TripsRoutingModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    SharedModule,
    MatDatepickerModule,

  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ResponseInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
  ],
  exports: [
    ScheduleTripComponent
  ]
})
export class TripsModule { }
