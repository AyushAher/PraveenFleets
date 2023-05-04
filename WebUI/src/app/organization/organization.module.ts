import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { OrganizationRoutingModule } from './organization-routing.module';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldModule } from '@angular/material/form-field';
import { ResponseInterceptor, ErrorInterceptor } from '../_helpers/interceptor';
import { JwtInterceptor } from '../_helpers/jwtinterceptor';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';
import { MatSelectModule } from '@angular/material/select';
import { RegisterCompanyComponent } from './register/register-company.component';
import { AgGridModule } from 'ag-grid-angular';
import { CreateEmployeeComponent } from './employee/create-employee.component';
import { CreateRolesComponent } from './roles/create-roles.component';
import { SharedModule } from '../shared/shared.module';
import { MatInputModule } from '@angular/material/input';
import { EmployeeListComponent } from './employee/employee-list.component';
import { OrganizationDashboardComponent } from './dashboard/dashboard.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { RegisterEmployeeComponent } from './register-employee/register-employee.component';
import { MatIconModule } from '@angular/material/icon';
import { ScheduleTripComponent } from './trips/schedule-trip.component';
import { TripsModule } from '../trips/trips.module';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { TripsListComponent } from './trips/trips-list.component';


@NgModule({
  declarations: [
    RegisterCompanyComponent,
    CreateEmployeeComponent,
    CreateRolesComponent,
    EmployeeListComponent,
    OrganizationDashboardComponent,
    RegisterEmployeeComponent,
    ScheduleTripComponent,
    TripsListComponent,
  ],
  imports: [
    CommonModule,
    OrganizationRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatStepperModule,
    MatFormFieldModule,
    AgGridModule,
    MatSelectModule,
    SharedModule,
    MatInputModule,
    MatSnackBarModule,
    MatIconModule,
    TripsModule,
    MatDatepickerModule
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
    { provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: { floatLabel: 'always' } }

  ],
})
export class OrganizationModule { }
