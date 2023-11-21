import {Component} from "@angular/core";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ReCapchaV3Service} from "../services/reCapchaV3.service";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {environment} from "../../environments/environment.prod";
import {firstValueFrom} from "rxjs";
import {ResponsdtoModel} from "../models/responsdto.model";
import {Globalstate} from "../services/states/globalstate";
import {AccountService, Credentials} from "../services/account.service";
import {TokenService} from "../services/token.service";
import {ToastController} from "@ionic/angular";
import {Router} from "@angular/router";

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
        <ion-checkbox data-action="login" (ionChange)="checkifhuman()" [formControl]="human">I'm not a robot</ion-checkbox>
      </ion-item>
      <ion-item lines="none" class="buttonitem">
        <ion-button color="primary" shape="round" (click)="login()" [disabled]="input.invalid && !this.state.ishuman || !this.state.ishuman || input.invalid">Login</ion-button>
        <ion-button color="primary" shape="round">sign-up</ion-button>
      </ion-item>


    </ion-content>
  `,
    styleUrls: ["login.component.style.scss"],
})

export class LoginComponent {
    email = new FormControl("", [Validators.required, Validators.minLength(6)]);
    password = new FormControl("", [Validators.required, Validators.minLength(8)]);
    human = new FormControl(false,[Validators.required]);
    input = new FormGroup(
      {
        email: this.email,
        password: this.password,
        })


    constructor(private recapcha: ReCapchaV3Service,
                private http: HttpClient,
                public state: Globalstate,
                private readonly service: AccountService,
                private readonly token: TokenService,
                private readonly toast: ToastController,
                private readonly router: Router,
                ) {
    }


  async checkifhuman() {
    const call = this.http.get<ResponsdtoModel>(environment.baseURL+"skey");
    const result = await firstValueFrom<ResponsdtoModel>(call);
    this.recapcha.execute(result.responseData.key)
  }


  async login() {
      try{
        const {token} = await firstValueFrom(this.service.login(this.input.value as Credentials));
        this.token.setToken(token);
        this.router.navigateByUrl('home');

        (await this.toast.create({
          message: "Login successful",
          color: "success",
          duration: 5000
        })).present();
    } catch (e)
      {
        (await this.toast.create({
          message: "Email or password was wrong please try again",
          color: "danger",
          duration: 5000,
        })).present();
      }
    }
}
