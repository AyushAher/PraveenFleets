import { Component, OnInit } from '@angular/core';
import OrganizationEmployeeService from '../_services/organization-employee.service';
import { ColDef, ColumnApi, GridApi } from 'ag-grid-community';
import OrganizationEmployeeResponse from 'src/app/_responses/organization-employee-response';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css']
})
export class EmployeeListComponent implements OnInit {
  public columnDefs: ColDef[];
  private columnApi: ColumnApi;
  private api: GridApi;
  List: OrganizationEmployeeResponse[];

  constructor(
    private organizationEmployeeService: OrganizationEmployeeService
  ) { }

  ngOnInit(): void {
    this.organizationEmployeeService.GetAllOrganizationEmployees()
      .subscribe(data => {
        console.log(data);

        this.List = data
      })

    this.columnDefs = this.createColumnDefs();

  }
  private createColumnDefs() {
    return [
      {
        headerName: "Name",
        field: "fullName",
        filter: true,
        enableSorting: true,
        sortable: true,
        tooltipField: "fullName",
      },
      {
        headerName: "Email",
        field: "email",
        filter: true,
        enableSorting: true,
        sortable: true,
        tooltipField: "email",
      },
      {
        headerName: "Role",
        field: "role",
        filter: true,
        sortable: true,
        tooltipField: "role",
      },
      {
        headerName: "Contact Number",
        field: "phoneNumber",
        filter: true,
        sortable: true,
        tooltipField: "phoneNumber",
      },
    ];
  }

  onGridReady(params: any): void {
    this.api = params.api;
    this.columnApi = params.columnApi;
    this.api.sizeColumnsToFit();
  }
}
