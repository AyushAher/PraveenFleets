import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import OrganizationService from '../_services/organization-service.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RegisterOrganizationRequest } from 'src/app/_requests/register-request';
import { AddressRequest } from 'src/app/_requests/address-request';
import { NotificationService } from 'src/app/_services/notification.service';

@Component({
  selector: 'app-register',
  templateUrl: './register-company.component.html',
})
export class RegisterCompanyComponent implements OnInit {
  OrganizationForm: FormGroup;
  AdminDetailsForm: FormGroup;
  AddressForm: FormGroup;

  constructor(
    _FormBuilder: FormBuilder,
    private organizationService: OrganizationService,
    private router: Router,
    private notificationService: NotificationService
  ) {
    this.OrganizationForm = _FormBuilder.group({
      name: ["", [Validators.required, Validators.minLength(1), Validators.maxLength(200)]],
      gstNumber: ["", [Validators.required, Validators.pattern("^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[0-9A-Z]{1}[Z]{1}[0-9A-Z]{1}$")]],
    });

    this.AddressForm = _FormBuilder.group({
      addressLine1: ["", [Validators.required, Validators.maxLength(250)]],
      addressLine2: ["", [Validators.required, Validators.maxLength(250)]],
      state: ["", [Validators.required, Validators.maxLength(100)]],
      city: ["", [Validators.required, Validators.maxLength(100)]],
      country: ["", [Validators.required, Validators.maxLength(100)]],
      pinCode: ["", [Validators.required, Validators.maxLength(8)]],
    });

    this.AdminDetailsForm = _FormBuilder.group({
      firstName: ["", [Validators.required, Validators.maxLength(60), Validators.minLength(1)]],
      lastName: ["", [Validators.required, Validators.maxLength(60), Validators.minLength(1)]],
      email: ["", [Validators.required, Validators.pattern("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]],
      phoneNumber: ["", [Validators.required]],
      password: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(20)]],
      confirmPassword: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(20)]],
      gender: ["", Validators.required],
      salutation: ["", Validators.required],
      weeklyOffs: ["", Validators.required],
      address: []
    })


  }

  ngOnInit(): void {
  }

  public get form() {
    return this.OrganizationForm.controls;
  }

  OnSubmit() {
    this.OrganizationForm.markAllAsTouched();
    this.AdminDetailsForm.markAllAsTouched();
    this.AddressForm.markAllAsTouched();

    if (this.OrganizationForm.invalid ||
      this.AdminDetailsForm.invalid ||
      this.AddressForm.invalid)
      return;

    var formData: RegisterOrganizationRequest = this.OrganizationForm.value;
    formData.addressRequest = this.AddressForm.value;
    formData.adminDetailsRequest = this.AdminDetailsForm.value;
    formData.adminDetailsRequest.gender = Number.parseInt(formData.adminDetailsRequest.gender)
    formData.adminDetailsRequest.salutation = Number.parseInt(formData.adminDetailsRequest.salutation)


    this.organizationService.Save(formData)
      .subscribe(data => {
        if (!data) return;
        this.notificationService.ShowSuccess("Organization Registered Successfully! Login to continue")
        this.router.navigate(["/user", "login"]);
      })
  }

}