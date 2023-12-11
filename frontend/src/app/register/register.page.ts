import {Component, OnInit} from "@angular/core";
import {AbstractControl, FormControl, FormGroup, ValidatorFn, Validators} from "@angular/forms";
import {firstValueFrom} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {newAccount} from "../accountInterface";
import {ToastController} from "@ionic/angular";
import {Router, UrlTree} from "@angular/router";
import {environment} from "../../environments/environment.prod";

@Component({
  selector: 'app-register',
  styleUrls: ['register.css'],
  template:
    `
      <ion-content>
        <ion-header>
          <ion-toolbar>
            <ion-title mode="ios">Register Account</ion-title>
          </ion-toolbar>
        </ion-header>
        <ion-grid>
          <ion-row>
            <ion-col>
              <ion-item >
              <ion-input [formControl]="AName" data-testid="accountName_" type="text" label-placement="floating" label="name" ></ion-input>
                <div *ngIf="AName.invalid && AName.touched" class="error">
                  Name must be between 1-100 letters
                </div>
              </ion-item>
              <ion-item >
                <ion-input [formControl]="AEmail" data-testid="accountEmail_" type="text" onpaste="return false;" ondrop="return false;" autocomplete="off" label-placement="floating" label="mail" ></ion-input>
                <div *ngIf="AEmail.invalid && AEmail.touched" class="error">
                  Must be a valid Email
                </div>
              </ion-item>
              <ion-item >
                <ion-input type="date" [formControl]="ADate"></ion-input>
              </ion-item>
              <ion-item >
                <ion-input [formControl]="APassword" data-testid="accountPassword_" type="text" onpaste="return false;" ondrop="return false;" autocomplete="off" label-placement="floating" label="password" [type]="hide ? 'password' : 'text'" required></ion-input>
                <ion-icon slot="end" [name]="hide ? 'eye' : 'eye-off'" (click)="hide = !hide"></ion-icon>
                <div *ngIf="APassword.invalid && APassword.touched" class="error">
                  Must be between8-32 carachters long and contain atleast 1 capital letter
                </div>
              </ion-item>
              <ion-item >
                <ion-input [formControl]="APasswordRepeat" data-testid="accountPasswordRepeat_" type="text" onpaste="return false;" ondrop="return false;" autocomplete="off" label-placement="floating" label="Repeat password" [type]="true ? 'password' : 'text'" required></ion-input>
                <div *ngIf="APasswordRepeat.invalid && APasswordRepeat.touched" class="error">
                  Both passwords must match
                </div>
              </ion-item>
              <ion-item>
                <ion-button (click)="createAccount()" data-testid="accountCreateBTN_"
                            *ngIf="APassword.valid && APasswordRepeat.valid && ADate.valid && AEmail.valid && AName.valid"
                >Create Account</ion-button>
              </ion-item>
            </ion-col>
            <ion-col>
              <ion-grid >
                <ion-row >
                  <ion-col class="grid-item">
                    <ion-img class="profile-img" style="width: 30%" [src]="currentAvatarUrl"/>
                  </ion-col>
                </ion-row>
                <ion-row>
                  <ion-col class="grid-item">
                    <ion-button>
                      Change Picture
                    </ion-button>
                  </ion-col>
                </ion-row>
              </ion-grid>
            </ion-col>
          </ion-row>
        </ion-grid>
      </ion-content>
        `,
})
export class RegisterPage {

  hide = true;
  defaultAvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png";
  currentAvatarUrl = this.defaultAvatarUrl;


  AName = new FormControl("",[Validators.required,Validators.minLength(1),Validators.maxLength(100)]);
  AEmail = new FormControl("",[Validators.required, Validators.email]);
  ADate = new FormControl(Date,[Validators.required,Validators.minLength(1),Validators.maxLength(31)]);
  APassword = new FormControl("",[Validators.required, Validators.minLength(8),Validators.maxLength(32)]);
  APasswordRepeat: FormControl = new FormControl("", [Validators.required, this.isPasswordMatching()]);

  private isPasswordMatching(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } | null => {
      if (this.APassword.value === this.APasswordRepeat.value) {
        return null;
      } else {
        return { 'passwordMismatch': true };
      }
    }
  }


  constructor(private http: HttpClient, public toastControl: ToastController, private router: Router) {

  }

  async createAccount() {
    try {
      const oberservable = this.http.post<newAccount>(environment.baseURL+'account/createuser', {
        userDisplayName: this.AName.value,
        userEmail: this.AEmail.value,
        userBirthday: this.ADate.value,
        password: this.APassword.value,

      });
      const result = await firstValueFrom<newAccount>(oberservable);
      this.toastControl.create(
        {
          color: "success",
          duration: 2000,
          message: "Success"
        }
      ).then(res =>{
        res.present();
      })
    }
    catch (error)
    {
      this.toastControl.create(
        {
          color: "warning",
          duration: 2000,
          message: "failed to create account"
        }
      ).then(res =>{
        res.present();
      })

    }
    this.router.navigateByUrl("login");
  }

}
