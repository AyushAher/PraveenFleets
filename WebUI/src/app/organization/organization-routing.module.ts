import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterCompanyComponent } from './register/register-company.component';
import { OrganizationDashboardComponent } from './dashboard/dashboard.component';
import { CreateRolesComponent } from './roles/create-roles.component';
import { EmployeeListComponent } from './employee/employee-list.component';
import { RegisterEmployeeComponent } from './register-employee/register-employee.component';
import { AuthGuard } from '../_helpers/auth.guard';
import { ScheduleTripComponent } from './trips/schedule-trip.component';
import { TripsListComponent } from './trips/trips-list.component';

const routes: Routes = [
  {
    path: "register",
    component: RegisterCompanyComponent
  },
  {
    path: "dashboard",
    component: OrganizationDashboardComponent,
    canActivate: [AuthGuard]
  },
  {
    path: "Employee/Create",
    component: RegisterEmployeeComponent,
    canActivate: [AuthGuard]
  },
  {
    path: "Employee",
    component: EmployeeListComponent,
    canActivate: [AuthGuard]
  },
  {
    path: "Roles/Create",
    component: CreateRolesComponent,
    canActivate: [AuthGuard]
  },
  {
    path: "Trips",
    component: TripsListComponent,
    canActivate: [AuthGuard]
  },
  {
    path: "Trips/Schedule",
    component: ScheduleTripComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrganizationRoutingModule { }
