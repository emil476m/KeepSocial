import { NgModule } from '@angular/core';
import {PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { HomePage } from './home/home.page';
import {TabsComponent} from "./tabs.component";
import {LoginComponent} from "./login/login.component";
import {RegisterPage} from "./register/register.page";
import {AccountPage} from "./account/account.page";

const routes: Routes = [
  {
    path: '',
    component: TabsComponent,
    children: [
      {
        path: "home",
        component: HomePage,
      },
    ]
  },
  {
    path: "login",
    component: LoginComponent,
  },
  {
    path: "register",
    component: RegisterPage,
  },
  {
    path: "account",
    component: AccountPage,
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {preloadingStrategy: PreloadAllModules})
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
