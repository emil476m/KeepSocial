import { Component } from '@angular/core';
import {TokenService} from "../services/token.service";
import {NavigationStart, Router} from "@angular/router";
import {ToastController} from "@ionic/angular";
import {AccountService} from "../services/account.service";
import {firstValueFrom} from "rxjs";
import {environment} from "../../environments/environment.prod";
import {HttpClient} from "@angular/common/http";
import {ResponsdtoModel} from "../models/responsdto.model";

@Component({
  selector: 'app-home',
  template: `
    <ion-toolbar>
      <ion-title align="center">KeepSocial</ion-title>
      <ion-buttons slot="end">
          <ion-button routerLink="/login" *ngIf="isloggedin == false">Login</ion-button>
        <ion-button routerLink="/register" *ngIf="isloggedin == false">Sign-up</ion-button>
        <ion-button (click)="logout()" *ngIf="isloggedin == true">
          <ion-icon name="logout"></ion-icon>
          logout
        </ion-button>
      </ion-buttons>
    </ion-toolbar>
    <ion-content>
      <ion-card>
        <ion-button (click)="check()">test</ion-button>
      </ion-card>
    </ion-content>
  `,
  styleUrls: ['home.page.scss'],
})
export class HomePage {
  isloggedin = false;


  constructor(public token: TokenService, private router : Router, private readonly toast: ToastController, private aService: AccountService, private http : HttpClient)
  {
    this.router.events.subscribe(event =>    {
      if(event instanceof NavigationStart) {
        this.checklogin();
      }
    })
  }

  checklogin() {
    if(this.token.getToken() != undefined)
    {
      this.isloggedin = true
    }
    else
    {
      this.isloggedin = false
    }
  }

  async logout() {
    try{
    this.token.clearToken();
      this.router.navigateByUrl('home');

    (await this.toast.create({
      message: "Logout successful",
      duration: 5000,
      color: 'success',
    })).present()
      this.checklogin();
    }
    catch (e)
    {
      (await this.toast.create(
        {
          message: "failed to logout",
          color: "danger",
          duration: 5000,
        }
      )).present();
    }
  }

  async check()
  {
    const call = this.http.get<ResponsdtoModel>(environment.baseURL+"account/getAllUsers");
    const result = await firstValueFrom(call);
    if(result != null)
    {
      console.log(result.messageToClient);
    }

  }
}


