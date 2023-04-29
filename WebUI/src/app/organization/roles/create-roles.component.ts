import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import OrganizationService from '../_services/organization-service.service';
import OrganizationResponse from 'src/app/_responses/Organization-response';
import OrganizationRoleService from '../_services/organization-role.service';
import { Router } from '@angular/router';
import { ColDef, ColumnApi, GridApi } from 'ag-grid-community';
import OrganizationRoleResponse from 'src/app/_responses/organization-role-response';
import { UserService } from 'src/app/user/_services/user-service.service';
import { UserResponse } from 'src/app/_responses/user-response';
import { NotificationService } from 'src/app/_services/notification.service';

@Component({
  selector: 'app-roles',
  templateUrl: './create-roles.component.html'
})
export class CreateRolesComponent implements OnInit {
  Form: FormGroup;
  Organization: OrganizationResponse;
  public columnDefs: ColDef[];
  private columnApi: ColumnApi;
  private api: GridApi;
  List: OrganizationRoleResponse[];
  user: UserResponse | null;

  constructor(
    _formBuilder: FormBuilder,
    organizationService: OrganizationService,
    private userService: UserService,
    private router: Router,
    private organizationRoleService: OrganizationRoleService,
    private notificationService: NotificationService
  ) {
    this.Form = _formBuilder.group({
      organization: ["", Validators.required],
      organizationId: ["", Validators.required],
      roleName: ["", Validators.required],
    })

    this.user = this.userService.userValue;

    organizationService.GetUserOrganizationDetails()
      .subscribe(data => {
        this.Organization = data
        if (data.name) {
          this.Form.get("organization")?.setValue(data.name)
          this.Form.get("organization")?.disable()
          this.Form.get("organizationId")?.setValue(data.id)
        }
      })

    this.GetAllOrgRoles();
  }

  GetAllOrgRoles() {
    if (!this.user) return;
    this.organizationRoleService.GetAllByOrganization(this.user.parentEntityId)
      .subscribe(organizationRoles => {
        this.List = organizationRoles
      });
  }

  ngOnInit(): void {

    this.columnDefs = this.createColumnDefs();

  }

  get f() {
    return this.Form.controls;
  }

  OnSubmit() {
    this.Form.markAllAsTouched();
    if (this.Form.invalid) return;

    this.organizationRoleService._object = this.Form.getRawValue()
    this.organizationRoleService.SaveRole()
      .subscribe(x => {
        if (!x) return;
        this.f.roleName.reset();
        this.notificationService.ShowSuccess("Role Saved Successfully");
        this.GetAllOrgRoles()
      })
  }


  private createColumnDefs() {
    return [
      {
        headerName: "Role",
        field: "roleName",
        filter: true,
        enableSorting: true,
        sortable: true,
        tooltipField: "roleName",
      },
    ];
  }

  onGridReady(params: any): void {
    this.api = params.api;
    this.columnApi = params.columnApi;
    this.api.sizeColumnsToFit();
  }

}
