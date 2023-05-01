import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, map, startWith } from 'rxjs';
import EnumResponse from 'src/app/_responses/enum-response';
import OrganizationEmployeeResponse from 'src/app/_responses/organization-employee-response';
import { AddressService } from 'src/app/_services/address.service';
import EnumService from 'src/app/_services/enum.service';
import { NotificationService } from 'src/app/_services/notification.service';
import OrganizationEmployeeService from 'src/app/organization/_services/organization-employee.service';
import { TripsService } from 'src/app/organization/_services/trips.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'schedule-trip',
  templateUrl: './schedule-trip.component.html',
  styleUrls: ['./schedule-trip.component.css']
})
export class ScheduleTripComponent implements OnInit {
  Form: FormGroup;
  pickUpAddressForm: FormGroup
  dropAddressForm: FormGroup
  employeeList: OrganizationEmployeeResponse[] = [];
  filteredOptions: Observable<OrganizationEmployeeResponse[]>;
  VehicleTypes: EnumResponse[];

  constructor(
    _formBuilder: FormBuilder,
    private notificationService: NotificationService,
    private enumService: EnumService,
    private organizationEmployeeService: OrganizationEmployeeService,
    private addressService: AddressService,
    private tripsService: TripsService,
  ) {
    this.Form = _formBuilder.group({
      office: [false, Validators.required],
      outStations: [false, Validators.required],
      vehicleType: ["", Validators.required],
      passengerEmailId: ["", [Validators.required]],
      passengerUserId: ["", [Validators.required]],
      passengerName: ["", Validators.required],
      pickupDate: ["", Validators.required],
      pickupTime: ["", Validators.required],
      dropDate: ["", Validators.required],
      dropTime: ["", Validators.required],
      pickUpAddress: ["", Validators.required],
      dropAddress: ["", Validators.required],
    });

    this.pickUpAddressForm = _formBuilder.group({
      addressLine1: ["", [Validators.required, Validators.maxLength(250)]],
      addressLine2: ["", [Validators.required, Validators.maxLength(250)]],
      state: ["", [Validators.required, Validators.maxLength(100)]],
      city: ["", [Validators.required, Validators.maxLength(100)]],
      country: ["", [Validators.required, Validators.maxLength(100)]],
      pinCode: ["", [Validators.required, Validators.maxLength(8)]],
    });

    this.dropAddressForm = _formBuilder.group({
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

    this.organizationEmployeeService.GetAllOrganizationEmployees()
      .subscribe(x => this.employeeList = x)

    this.enumService.GetEnumList(environment.enumNames.VehicleTypes)
      .subscribe(x => this.VehicleTypes = x);


    this.filteredOptions = this.Form.controls.passengerEmailId
      .valueChanges.pipe(
        startWith(''), map(value => {
          const email = typeof value === 'string' ? value : value?.email;
          return email ? this._filter(email as string) : this.employeeList.slice();
        }),
      );

    this.Form.controls.passengerEmailId.valueChanges
      .subscribe(value => {
        if (typeof value === 'string') {
          value = this.employeeList.find(x => x.email == value)
          if (!value) return;
        }

        this.f.passengerName.disable();
        this.f.passengerName.setValue(value?.fullName);
        this.f.passengerUserId.setValue(value?.userId);
        this.pickUpAddressForm.patchValue(value?.address)
      })

    this.Form.controls.office.valueChanges
      .subscribe(value => {
        if (!value) {
          this.dropAddressForm.reset()
          this.dropAddressForm.enable();
          return;
        }

        this.dropAddressForm.disable();
        this.addressService.GetAddressByCurrentUserParentId()
          .subscribe(x => this.dropAddressForm.patchValue(x));

      })
  }

  get f() {
    return this.Form.controls
  }

  displayFn(user: OrganizationEmployeeResponse): string {
    return user && (user.email ?? "");
  }

  private _filter(email: string): OrganizationEmployeeResponse[] {
    const filterValue = email.toLowerCase();

    return this.employeeList.filter(option => option.email.toLowerCase().includes(filterValue));
  }


  onSaveAsDraft() {
    this.Form.markAllAsTouched();
    this.pickUpAddressForm.markAllAsTouched();
    this.dropAddressForm.markAllAsTouched();

    if (this.Form.invalid ||
      this.pickUpAddressForm.invalid ||
      this.dropAddressForm.invalid) return this.notificationService.ShowInfo("Please fill all required fields");

    let formData = this.Form.getRawValue();
    formData.vehicleType = Number.parseInt(formData.vehicleType)
    formData.passengerEmailId = formData.passengerEmailId.email

    this.tripsService.SaveTripDraft(formData)
      .subscribe()
  }

}