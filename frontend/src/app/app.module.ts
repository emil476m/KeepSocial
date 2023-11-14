import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouteReuseStrategy } from '@angular/router';

import { IonicModule, IonicRouteStrategy } from '@ionic/angular';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import {HomePage} from "./home/home.page";
import {TabsComponent} from "./tabs.component";
import {ErrorHttpInterceptor} from "../interceptors/error-http-interceptors";
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {TokenService} from "./services/token.service";
import {AuthHttpInterceptor} from "../interceptors/auth-http-interceptor";
import {LoginComponent} from "./login/login.component";
import {ReactiveFormsModule} from "@angular/forms";

@NgModule({
  declarations: [AppComponent, HomePage, TabsComponent, LoginComponent],
    imports: [BrowserModule, IonicModule.forRoot(), AppRoutingModule, ReactiveFormsModule],
  providers: [{ provide: RouteReuseStrategy, useClass: IonicRouteStrategy }, { provide: HTTP_INTERCEPTORS, useClass: ErrorHttpInterceptor, multi: true },{ provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true },TokenService],
  bootstrap: [AppComponent],
})
export class AppModule {}
