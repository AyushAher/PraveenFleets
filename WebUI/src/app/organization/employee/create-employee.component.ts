import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import EnumService from 'src/app/_services/enum.service';
import EnumResponse from 'src/app/_responses/enum-response';
import { environment } from 'src/environments/environment';
import OrganizationRoleService from '../_services/organization-role.service';
import { UserService } from 'src/app/user/_services/user-service.service';
import { UserResponse } from 'src/app/_responses/user-response';
import OrganizationRoleResponse from 'src/app/_responses/organization-role-response';
import OrganizationEmployeeService from '../_services/organization-employee.service';


@Component({
  selector: 'organization-create-employee',
  templateUrl: './create-employee.component.html',
  styleUrls: ['./create-employee.component.css']
})
export class CreateEmployeeComponent implements OnInit {
  @Input("IsAdmin") IsAdmin: boolean;
  @Output() EmployeeFormChange: EventEmitter<FormGroup> = new EventEmitter();
  @Input("EmployeeForm") EmployeeForm: FormGroup;
  AddressForm: FormGroup;

  user: UserResponse | null;
  GenderList: EnumResponse[];
  SalutationList: EnumResponse[];
  WeekDaysList: EnumResponse[];
  weeklyOffsControl: FormControl = new FormControl("", Validators.required);
  RoleList: OrganizationRoleResponse[];

  constructor(
    _FormBuilder: FormBuilder,
    private enumService: EnumService,
    private organizationRoleService: OrganizationRoleService,
    private userService: UserService,
    private organizationEmployeeService: OrganizationEmployeeService
  ) {
    this.EmployeeForm = _FormBuilder.group({
      firstName: ["", [Validators.required, Validators.maxLength(60), Validators.minLength(1)]],
      lastName: ["", [Validators.required, Validators.maxLength(60), Validators.minLength(1)]],
      email: ["", [Validators.required, Validators.pattern("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]],
      phoneNumber: ["", [Validators.required]],
      password: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(20)]],
      confirmPassword: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(20)]],
      roleId: [""],
      gender: ["", Validators.required],
      salutation: ["", Validators.required],
      weeklyOffs: ["", Validators.required],
      address: ["", Validators.required]
    })


    this.AddressForm = _FormBuilder.group({
      addressLine1: ["", [Validators.required, Validators.maxLength(250)]],
      addressLine2: ["", [Validators.required, Validators.maxLength(250)]],
      state: ["", [Validators.required, Validators.maxLength(100)]],
      city: ["", [Validators.required, Validators.maxLength(100)]],
      country: ["", [Validators.required, Validators.maxLength(100)]],
      pinCode: ["", [Validators.required, Validators.maxLength(8)]],
    });

    if (this.IsAdmin) this.EmployeeForm.get("roleId")?.setValidators([Validators.required])

    else this.EmployeeForm.get("password")?.valueChanges
      .subscribe(value => this.EmployeeForm.get("confirmPassword")?.setValue(value))


    this.weeklyOffsControl.valueChanges
      .subscribe(value => this.employee.weeklyOffs.setValue(value))

    this.user = this.userService.userValue;
  }

  ngOnInit(): void {
    this.EmployeeForm.valueChanges
      .subscribe(_ => this.EmployeeFormChange.emit(this.EmployeeForm))

    this.enumService.GetEnumList(environment.enumNames.Gender)
      .subscribe(x => this.GenderList = x);

    this.enumService.GetEnumList(environment.enumNames.Salutation)
      .subscribe(x => this.SalutationList = x);

    this.enumService.GetEnumList(environment.enumNames.WeekDays)
      .subscribe(x => this.WeekDaysList = x);

    this.AddressForm.valueChanges
      .subscribe(value => this.EmployeeForm.patchValue({ 'address': value }))



    if (this.user)
      this.organizationRoleService.GetAllByOrganization(this.user.parentEntityId)
        .subscribe(organizationRoles => this.RoleList = organizationRoles)

  }

  public get employee() {
    return this.EmployeeForm.controls;
  }

  OnSubmit() {
    let formData = this.EmployeeForm.getRawValue();
    formData.gender = Number.parseInt(formData.gender)
    formData.salutation = Number.parseInt(formData.salutation)
    console.log(formData);

    this.organizationEmployeeService.RegisterEmployee(formData)
      .subscribe()

  }
}
