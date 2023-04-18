import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { HomeComponent } from "./home/home.component";

const routes: Routes = [
    {
        path: "user",
        loadChildren: () => import('./user/user.module').then(m => m.UserModule)
    },
    {
        path: "",
        component: HomeComponent
    },
    {
        path: "dashboard",
        loadChildren: () => import("./dashboard/dashboard.module").then(x => x.DashboardModule)
    }
]; // sets up routes constant where you define your routes

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export default class AppRoutingModule { }