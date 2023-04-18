import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { OrganizationDashboardComponent } from './organization-dashboard/organization.component';
import { CustomerSuppDashboardComponent } from './customer-supp-dashboard/customer-supp-dashboard.component';


@NgModule({
  declarations: [
    OrganizationDashboardComponent,
    CustomerSuppDashboardComponent
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule
  ]
})
export class DashboardModule { }
