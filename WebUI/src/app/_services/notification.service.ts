import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, Subject, config } from 'rxjs';
import { SnackbarComponent } from '../shared/snackbar/snackbar.component';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private _listeners = new Subject<any>();

  constructor(private _snackBar: MatSnackBar) { }


  ShowSuccess(body: string, title: string = "Success") {
    this._snackBar.openFromComponent(SnackbarComponent, {
      data: {
        message: body,
        icon: 'Success'
      }
    });
    console.log(title, body);
  }

  ShowError(body: string | string[], title: string = "Error") {
    this._snackBar.openFromComponent(SnackbarComponent, {
      data: {
        message: body,
        icon: 'Error'
      }
    });
  }

  ShowInfo(body: string, title: string = "Info") {
    this._snackBar.openFromComponent(SnackbarComponent, {
      data: {
        message: body,
        icon: 'Info'
      }
    });
  }


  Listen(): Observable<any> {
    return this._listeners.asObservable();
  }

  Emit(filterBy: string) {
    this._listeners.next(filterBy);
  }

  ShowWarning(body: string, title: string = "Warning",) {
    this._snackBar.openFromComponent(SnackbarComponent, {
      data: {
        message: body,
        icon: 'Warn'
      }
    });
  }
}
