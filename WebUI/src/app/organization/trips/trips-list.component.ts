import { Component, OnInit } from '@angular/core';
import { TripsService } from '../_services/trips.service';
import TripsResponse from 'src/app/_responses/trips-response';
import { ColDef, ColumnApi, GridApi } from 'ag-grid-community';

@Component({
  selector: 'app-trips-list',
  templateUrl: './trips-list.component.html'
})
export class TripsListComponent implements OnInit {
  public columnDefs: ColDef[];
  private api: GridApi;
  List: TripsResponse[];

  constructor(private tripsService: TripsService) { }

  ngOnInit(): void {
    this.tripsService.GetTripsByCurrentOrg()
      .subscribe(data => {
        data.forEach(x => {
          x.dropDate = new Date(x.dropDate).toLocaleDateString("en-GB");
          x.pickUpDate = new Date(x.pickUpDate).toLocaleDateString("en-GB");
        });

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
        field: "pickUpDate",
        filter: true,
        enableSorting: true,
        sortable: true,
        tooltipField: "pickUpDate",
      },
      {
        field: "dropDate",
        filter: true,
        enableSorting: true,
        sortable: true,
        tooltipField: "dropDate",
      },
      {
        headerName: "Vehicle Type",
        field: "vehicleTypeDesc",
        filter: true,
        sortable: true,
        tooltipField: "vehicleTypeDesc",
      },
      {
        headerName: "Status",
        field: "statusDesc",
        filter: true,
        sortable: true,
        tooltipField: "statusDesc",
      },
    ];
  }

  onGridReady(params: any): void {
    this.api = params.api;
    this.api.sizeColumnsToFit();
  }
}
