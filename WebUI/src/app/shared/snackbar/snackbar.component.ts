import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-snackbar',
  templateUrl: './snackbar.component.html'
})
export class SnackbarComponent {
  titleColor: any = {};
  constructor(@Inject(MAT_SNACK_BAR_DATA) public data: any) {

    switch (data?.icon) {
      case "Success":
        this.titleColor.color = "green";
        break;

      case "Error":
        this.titleColor.color = "red";
        break;

      case "Info":
        this.titleColor.color = "blue";
        break;

      case "Warn":
        this.titleColor.color = "yellow";
        break;

      default:
        break;
    }
  }
}
