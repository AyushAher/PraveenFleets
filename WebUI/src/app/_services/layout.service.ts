import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable({ providedIn: "any" })
export default class LayoutService {
    sideBarOpen: BehaviorSubject<boolean>;
    isSideBarOpen: boolean = true;

    navbarState: BehaviorSubject<boolean>;
    showNavbar: boolean = true;

    constructor() {
        this.sideBarOpen = new BehaviorSubject<boolean>(true);
        this.navbarState = new BehaviorSubject<boolean>(true);
    }

    ToggleSidebar() {
        this.isSideBarOpen = !this.isSideBarOpen
        this.sideBarOpen.next(this.isSideBarOpen);
    }

    get SideNavbarObserver(): Observable<boolean> {
        return this.sideBarOpen.asObservable();
    }

    ToggleNavbar() {
        this.showNavbar = !this.showNavbar
        this.navbarState.next(this.showNavbar);
    }

    get NavbarObserver(): Observable<boolean> {
        return this.navbarState.asObservable();
    }

}