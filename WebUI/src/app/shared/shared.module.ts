import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddressComponent } from './address/address.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ResponseInterceptor, ErrorInterceptor } from '../_helpers/interceptor';
import { JwtInterceptor } from '../_helpers/jwtinterceptor';
import { LayoutComponent } from './layout/layout.component';
import { NavBarComponent } from './layout/nav-bar/nav-bar.component';
import { SideNavBarComponent } from './layout/side-nav-bar/side-nav-bar.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterModule } from '@angular/router';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { SnackbarComponent } from './snackbar/snackbar.component';


@NgModule({
  declarations: [
    AddressComponent,
    LayoutComponent,
    NavBarComponent,
    SideNavBarComponent,
    SnackbarComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    RouterModule,
    MatSnackBarModule,
    MatIconModule
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
    AddressComponent,
    LayoutComponent,
    NavBarComponent,
    SideNavBarComponent,
  ]
})
export class SharedModule { }
