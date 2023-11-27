import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouteReuseStrategy } from '@angular/router';

import { IonicModule, IonicRouteStrategy } from '@ionic/angular';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import {HomePage} from "./home/home.page";
import {TabsComponent} from "./tabs.component";
import {ErrorHttpInterceptor} from "../interceptors/error-http-interceptors";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {TokenService} from "./services/token.service";
import {AuthHttpInterceptor} from "../interceptors/auth-http-interceptor";
import {LoginComponent} from "./login/login.component";
import {ReactiveFormsModule} from "@angular/forms";
import {NgxCaptchaModule} from "ngx-captcha";
import {ReCapchaV3Service} from "./services/reCapchaV3.service";
import {RegisterPage} from "./register/register.page";
import {AccountService} from "./services/account.service";
import {ChatPage} from "./chat/chat.page";
import {AccountPage} from "./account/account.page";
import {NewAccountInfoModal} from "./changeAccountInfoModal/AccountInfoModal";

@NgModule({
  declarations:
    [AppComponent,
    HomePage,
    TabsComponent,
    LoginComponent,
    RegisterPage,
    AccountPage,
    ChatPage,
    NewAccountInfoModal,
  ],
    imports:
      [BrowserModule,
      IonicModule.forRoot(),
      AppRoutingModule,
      ReactiveFormsModule,
      NgxCaptchaModule,
      HttpClientModule,
    ],
  providers: [{ provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorHttpInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true },
    TokenService,
    ReCapchaV3Service,
    AccountService,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
