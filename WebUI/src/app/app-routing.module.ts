import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { HomeComponent } from "./home/home.component";
import { NotFoundComponent } from "./not-found/not-found.component";
import { AuthGuard } from "./_helpers/auth.guard";

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
        component: HomeComponent,
        canActivate: [AuthGuard]
    },
    {
        path: "**",
        component: NotFoundComponent
    }
]; // sets up routes constant where you define your routes

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export default class AppRoutingModule { }