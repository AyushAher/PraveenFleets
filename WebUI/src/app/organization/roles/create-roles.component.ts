import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import OrganizationService from '../_services/organization-service.service';
import OrganizationResponse from 'src/app/_responses/Organization-response';
import OrganizationRoleService from '../_services/organization-role.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-roles',
  templateUrl: './create-roles.component.html'
})
export class CreateRolesComponent implements OnInit {
  Form: FormGroup;
  Organization: OrganizationResponse;

  constructor(
    _formBuilder: FormBuilder,
    organizationService: OrganizationService,
    private router: Router,
    private organizationRoleService: OrganizationRoleService
  ) {
    this.Form = _formBuilder.group({
      organization: ["", Validators.required],
      organizationId: ["", Validators.required],
      roleName: ["", Validators.required],
    })

    organizationService.GetUserOrganizationDetails()
      .subscribe(data => {
        this.Organization = data
        if (data.name) {
          this.Form.get("organization")?.setValue(data.name)
          this.Form.get("organization")?.disable()
          this.Form.get("organizationId")?.setValue(data.id)
        }
      })

  }

  ngOnInit(): void {


  }

  get f() {
    return this.Form.controls;
  }

  OnSubmit() {
    this.organizationRoleService._object = this.Form.getRawValue()
    this.organizationRoleService.SaveRole()
      .subscribe(x => this.router.navigate(['']))
  }



}
