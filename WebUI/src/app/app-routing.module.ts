import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { HomeComponent } from "./home/home.component";

const routes: Routes = [
    {
        path: "user",
        loadChildren: () => import('./user/user.module').then(m => m.UserModule)
    },
    {
        path: "organization",
        loadChildren: () => import("./organization/organization.module").then(x => x.OrganizationModule)
    },
    {
        path: "",
        component: HomeComponent
    },
    {
        path: "",
        component: HomeComponent
    }
]; // sets up routes constant where you define your routes

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export default class AppRoutingModule { }