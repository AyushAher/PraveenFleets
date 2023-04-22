import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterCompanyComponent } from './register/register-company.component';
import { OrganizationDashboardComponent } from './dashboard/dashboard.component';
import { CreateEmployeeComponent } from './employee/create-employee.component';
import { CreateRolesComponent } from './roles/create-roles.component';

const routes: Routes = [
  {
    path: "register",
    component: RegisterCompanyComponent
  },
  {
    path: "dashboard",
    component: OrganizationDashboardComponent
  },
  {
    path: "Employee/Create",
    component: CreateEmployeeComponent
  },
  {
    path: "Roles/Create",
    component: CreateRolesComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrganizationRoutingModule { }
