import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { ThemePalette } from '@angular/material/core';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { SpinnerService } from '../_services/sipnner.service';
import { ActivatedRoute } from '@angular/router';
import LayoutService from '../_services/layout.service';

@Component({
  selector: 'layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent implements OnInit {
  mode: ProgressSpinnerMode = 'indeterminate';
  color: ThemePalette = 'accent';
  showSpinner = false;
  showSidebar: boolean;
  showNavbar: boolean;

  @Input("Title") Title: string;

  constructor(
    private spinnerService: SpinnerService,
    private cdRef: ChangeDetectorRef,
    private layoutService: LayoutService
  ) { }

  ngOnInit(): void {

    this.layoutService.NavbarObserver
      .subscribe((data: any) => {
        this.showNavbar = data
        this.cdRef.detectChanges();
      })

    this.layoutService.SideNavbarObserver
      .subscribe((data: any) => {
        this.showSidebar = data
        this.cdRef.detectChanges();
      })

    this.spinnerService.getSpinnerObserver()
      .subscribe((status) => {
        this.showSpinner = (status === 'start');
        this.cdRef.detectChanges();
      });
  }

}
