import {Component} from "@angular/core";
import {FormControl, Validators} from "@angular/forms";

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
      <ion-item lines="none" class="buttonitem">
        <ion-button color="primary" shape="round">Login</ion-button>
        <ion-button color="primary" shape="round">sign-up</ion-button>
      </ion-item>



    </ion-content>
  `,
  styleUrls: ["login.component.style.scss"]
})

export class LoginComponent
{
  email = new FormControl("", [Validators.required, Validators.minLength(6)]);
  password = new FormControl("", [Validators.required, Validators.minLength(8)]);

  protected readonly screenX = screenX;
  protected readonly screenY = screenY;
}
