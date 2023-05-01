import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'schedule-trip',
  templateUrl: './schedule-trip.component.html',
  styleUrls: ['./schedule-trip.component.css']
})
export class ScheduleTripComponent implements OnInit {
  Form: FormGroup;
  pickUpAddressForm: FormGroup
  dropAddressForm: FormGroup

  constructor(_formBuilder: FormBuilder) {
    this.Form = _formBuilder.group({
      office: [false, Validators.required],
      outStations: [false, Validators.required],
      vehicleType: ["", Validators.required],
      passengerEmailId: ["", [Validators.required, Validators.email]],
      passengerName: ["", Validators.required],
      pickupDate: ["", Validators.required],
      pickupTime: ["", Validators.required],
      dropDate: ["", Validators.required],
      dropTime: ["", Validators.required],
      pickUpAddress: ["", Validators.required],
      dropAddress: ["", Validators.required],
    });

    this.pickUpAddressForm = this.dropAddressForm = _formBuilder.group({
      addressLine1: ["", [Validators.required, Validators.maxLength(250)]],
      addressLine2: ["", [Validators.required, Validators.maxLength(250)]],
      state: ["", [Validators.required, Validators.maxLength(100)]],
      city: ["", [Validators.required, Validators.maxLength(100)]],
      country: ["", [Validators.required, Validators.maxLength(100)]],
      pinCode: ["", [Validators.required, Validators.maxLength(8)]],
    });
  }

  ngOnInit(): void {
    this.pickUpAddressForm.valueChanges
      .subscribe(value => this.Form.patchValue({ 'pickUpAddress': value }))

    this.dropAddressForm.valueChanges
      .subscribe(value => this.Form.patchValue({ 'dropAddress': value }))

  }

}