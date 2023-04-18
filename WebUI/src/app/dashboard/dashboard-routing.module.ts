import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrganizationDashboardComponent } from './organization-dashboard/organization.component';
import { CustomerSuppDashboardComponent } from './customer-supp-dashboard/customer-supp-dashboard.component';

const routes: Routes = [
  {
    path: "organization",
    component: OrganizationDashboardComponent
  },
  {
    path: "customerSupport",
    component: CustomerSuppDashboardComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
