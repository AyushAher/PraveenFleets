import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FloatLabelType } from '@angular/material/form-field';

@Component({
  selector: 'shared-address',
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.css']
})
export class AddressComponent implements OnInit {
  @Output() AddressFormChange: EventEmitter<FormGroup> = new EventEmitter();
  @Input("IsAdmin") IsAdmin: boolean;
  @Input("AddressForm") AddressForm: FormGroup;

  floatLabelControl = new FormControl('auto' as FloatLabelType);

  constructor(formBuilder: FormBuilder) {
    this.AddressForm = formBuilder.group({
      addressLine1: ["", [Validators.required, Validators.maxLength(250)]],
      addressLine2: ["", [Validators.required, Validators.maxLength(250)]],
      state: ["", [Validators.required, Validators.maxLength(100)]],
      city: ["", [Validators.required, Validators.maxLength(100)]],
      country: ["", [Validators.required, Validators.maxLength(100)]],
      pinCode: ["", [Validators.required, Validators.maxLength(8)]],
    })
  }

  ngOnInit(): void {
    this.AddressForm.valueChanges
      .subscribe(_ => this.AddressFormChange.emit(this.AddressForm))
  }

  getFloatLabelValue(): FloatLabelType {
    return this.floatLabelControl.value || 'auto';
  }

  get address() {
    return this.AddressForm.controls;
  }
}
