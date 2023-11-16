import {Component} from "@angular/core";
import {FormControl, Validators} from "@angular/forms";
import {ReCapchaV3Service} from "../services/reCapchaV3.service";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment.prod";
import {firstValueFrom} from "rxjs";
import {ResponsdtoModel} from "../models/responsdto.model";
import {Globalstate} from "../services/states/globalstate";

@Component({
    template: `
    <ion-toolbar style="--background: none;">
      <ion-title align="center">Login</ion-title>
    </ion-toolbar>
    <ion-content>

      <ion-item class="text" lines="none" style="--background: none;  margin-top: 20px;">
        <ion-input type="email" [formControl]="email" label="Email" labelPlacement="floating"></ion-input>
        <div *ngIf="email.invalid && email.touched">
          Email is required
        </div>
      </ion-item>
      <ion-item class="text" lines="none" style="--background: none;  margin-top: 20px;">
        <ion-input class="input" type="password" [formControl]="password" labelPlacement="floating" label="Password"></ion-input>
        <div *ngIf="password.invalid && password.touched">
          Password is required
        </div>
      </ion-item>
      <ion-item lines="none" class="ishuman">
        <ion-checkbox data-action="login" (ionChange)="checkifhuman()">I'm human</ion-checkbox>
      </ion-item>
      <ion-item lines="none" class="buttonitem">
        <ion-button color="primary" shape="round">Login</ion-button>
        <ion-button color="primary" shape="round">sign-up</ion-button>
      </ion-item>


    </ion-content>
  `,
    styleUrls: ["login.component.style.scss"],
})

export class LoginComponent {
    email = new FormControl("", [Validators.required, Validators.minLength(6)]);
    password = new FormControl("", [Validators.required, Validators.minLength(8)]);

    constructor(private recapcha: ReCapchaV3Service, private http: HttpClient, private state: Globalstate) {
    }


  async checkifhuman() {
    const call = this.http.get<ResponsdtoModel>(environment.baseURL+"skey");
    const result = await firstValueFrom<ResponsdtoModel>(call);
    this.recapcha.execute(result.responseData.key)
  }
}
