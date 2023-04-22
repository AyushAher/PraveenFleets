import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import OrganizationService from '../_services/organization-service.service';
import OrganizationResponse from 'src/app/_responses/Organization-response';

@Component({
  selector: 'app-roles',
  templateUrl: './create-roles.component.html'
})
export class CreateRolesComponent implements OnInit {
  Form: FormGroup;
  Organization: OrganizationResponse;

  constructor(
    _formBuilder: FormBuilder,
    organizationService: OrganizationService
  ) {
    this.Form = _formBuilder.group({
      organization: ["", Validators.required],
      organizationId: ["", Validators.required],
      roleName: ["", Validators.required],
    })

    organizationService.GetUserOrganizationDetails()
      .subscribe(data => this.Organization = data)

  }

  ngOnInit(): void {

    if (this.Organization.name) {
      this.Form.get("organization")?.setValue(this.Organization.name)
      this.Form.get("organizationId")?.setValue(this.Organization.id)

    }


  }

  get f() {
    return this.Form.controls;
  }

}
