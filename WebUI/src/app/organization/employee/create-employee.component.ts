import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import EnumService from 'src/app/_services/enum.service';
import EnumResponse from 'src/app/_responses/enum-response';
import { environment } from 'src/environments/environment';
import OrganizationRoleService from '../_services/organization-role.service';
import { UserService } from 'src/app/user/_services/user-service.service';
import { UserResponse } from 'src/app/_responses/user-response';
import OrganizationRoleResponse from 'src/app/_responses/organization-role-response';

@Component({
  selector: 'organization-create-employee',
  templateUrl: './create-employee.component.html',
  styleUrls: ['./create-employee.component.css']
})
export class CreateEmployeeComponent implements OnInit {
  @Input("IsAdmin") IsAdmin: boolean;
  user: UserResponse | null;
  EmployeeDetailsForm: FormGroup;
  GenderList: EnumResponse[];
  SalutationList: EnumResponse[];
  WeekDaysList: EnumResponse[];
  RoleList: OrganizationRoleResponse[];
  weeklyOffsControl: FormControl = new FormControl("", Validators.required);

  constructor(
    _FormBuilder: FormBuilder,
    private enumService: EnumService,
    private organizationRoleService: OrganizationRoleService,
    private userService: UserService
  ) {
    this.EmployeeDetailsForm = _FormBuilder.group({
      firstName: ["", [Validators.required, Validators.maxLength(60), Validators.minLength(1)]],
      lastName: ["", [Validators.required, Validators.maxLength(60), Validators.minLength(1)]],
      email: ["", [Validators.required, Validators.pattern("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]],
      phoneNumber: ["", [Validators.required]],
      password: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(20)]],
      confirmPassword: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(20)]],
      role: ["", Validators.required],
      gender: ["", Validators.required],
      salutation: ["", Validators.required],
      weekDays: ["", Validators.required]
    })

    this.weeklyOffsControl.valueChanges
      .subscribe(value => this.employee.weekDays.setValue(value))

    this.user = this.userService.userValue;
  }

  ngOnInit(): void {


    if (this.IsAdmin) {
      this.EmployeeDetailsForm.get("role")?.setValue("Admin")
      this.EmployeeDetailsForm.get("role")?.disable()
    }

    this.enumService.GetEnumList(environment.enumNames.Gender)
      .subscribe(x => this.GenderList = x);

    this.enumService.GetEnumList(environment.enumNames.Salutation)
      .subscribe(x => this.SalutationList = x);

    this.enumService.GetEnumList(environment.enumNames.WeekDays)
      .subscribe(x => this.WeekDaysList = x);


    if (this.user)
      this.organizationRoleService.GetAllByOrganization(this.user.parentEntityId)
        .subscribe(organizationRoles => this.RoleList = organizationRoles)

  }

  public get employee() {
    return this.EmployeeDetailsForm.controls;
  }

  OnSubmit() {
    console.log(this.EmployeeDetailsForm.value);

  }
}
