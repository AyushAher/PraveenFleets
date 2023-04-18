import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private _listeners = new Subject<any>();

  constructor() { }

  ShowSuccess(body: string, title: string = "Success") {
    console.log(title, body);
  }

  ShowError(body: string, title: string = "Error") {
    console.log(title, body);
  }

  ShowInfo(body: string, title: string = "Info") {
    console.log(title, body);
  }


  Listen(): Observable<any> {
    return this._listeners.asObservable();
  }

  Emit(filterBy: string) {
    this._listeners.next(filterBy);
  }

  ShowWarning(body: string, title: string = "Warning",) {
    console.log(title, body);
  }
}
