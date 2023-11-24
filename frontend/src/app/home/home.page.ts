import { Component } from '@angular/core';
import {TokenService} from "../services/token.service";
import {NavigationStart, Router} from "@angular/router";
import {ToastController} from "@ionic/angular";
import {AccountService} from "../services/account.service";
import {firstValueFrom} from "rxjs";
import {environment} from "../../environments/environment.prod";
import {HttpClient} from "@angular/common/http";
import {ResponsdtoModel} from "../models/responsdto.model";
import {FormControl, Validators} from "@angular/forms";

@Component({
  selector: 'app-home',
  template: `
    <ion-toolbar>
      <ion-title align="center">KeepSocial</ion-title>
      <ion-buttons slot="end">
          <ng-template #notLoggedin>
          <ion-button routerLink="/login">
              <ion-icon name="log-in-outline"></ion-icon>
              Login
          </ion-button>
        <ion-button routerLink="/register">Sign-up</ion-button>
          </ng-template>
        <ion-button (click)="logout()" *ngIf="token.getToken(); else notLoggedin">
          <ion-icon name="log-out-outline"></ion-icon>
          logout
        </ion-button>
      </ion-buttons>
    </ion-toolbar>
    <ion-content>
      <ion-card *ngIf="token.getToken()">
        <ion-toolbar></ion-toolbar>
          <ion-textarea [counter]="true" [maxlength]="500" placeholder="what do you want your post to say?" [formControl]="textFC"></ion-textarea>
          <div>
              <ion-input placeholder="image url" [formControl]="imageFC"></ion-input>
          </div>
          <ion-buttons>
              <ion-button>post</ion-button>
          </ion-buttons>
      </ion-card>
    </ion-content>
  `,
  styleUrls: ['home.page.scss'],
})
export class HomePage {

  textFC = new FormControl("",[Validators.required, Validators.maxLength(500), Validators.minLength(0)]);
  imageFC = new FormControl("");


  constructor(public token: TokenService, private router : Router, private readonly toast: ToastController, private aService: AccountService, private http : HttpClient)
  {}


  async logout() {
    try{
    this.token.clearToken();

    (await this.toast.create({
      message: "Logout successful",
      duration: 5000,
      color: 'success',
    })).present()
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

}


