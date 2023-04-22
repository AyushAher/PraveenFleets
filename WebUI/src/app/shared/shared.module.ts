import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from '../home/home.component';
import { AddressComponent } from './address/address.component';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';



@NgModule({
  declarations: [
    AddressComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
  ],
  exports: [AddressComponent]
})
export class SharedModule { }
