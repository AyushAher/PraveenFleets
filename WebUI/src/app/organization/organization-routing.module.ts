import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterCompanyComponent } from './register/register-company.component';
import { OrganizationDashboardComponent } from './dashboard/dashboard.component';

const routes: Routes = [
  {
    path: "register",
    component: RegisterCompanyComponent
  },
  {
    path: "dashboard",
    component: OrganizationDashboardComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrganizationRoutingModule { }
