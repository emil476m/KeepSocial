import {Component, OnInit} from "@angular/core";
import {AbstractControl, FormControl, Validators} from "@angular/forms";
import {firstValueFrom} from "rxjs";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {newAccount} from "../accountInterface";
import {ToastController} from "@ionic/angular";
import {calendarNumber} from "ionicons/icons";

@Component({
  selector: 'app-boxdetailed',
  styleUrls: ['register.css'],
  template:
    `


      <ion-content>

        <ion-header>
          <ion-toolbar>
            <ion-title align="center">Register Account</ion-title>
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
                <ion-input [formControl]="AYear" data-testid="accountYear_" type="number" label-placement="floating" label="Year" ></ion-input>
                <ion-input [formControl]="AMonth" data-testid="accountMonth_" type="number" label-placement="floating" label="Month" ></ion-input>
                <ion-input [formControl]="ADay" data-testid="accountDay_" type="number" label-placement="floating" label="Day" ></ion-input>
                <div *ngIf="AYear.invalid && AYear.touched && AMonth.invalid && AMonth.touched && ADay.invalid && ADay.touched" class="error">
                  Please enter a valid date
                </div>
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

            </ion-col>

            <ion-col>
              <ion-grid >
                <ion-row >
                  <ion-col class="grid-item">
                    <ion-img class="profile-img" style="width: 30%" src='https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png'/>
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
export class RegisterPage implements OnInit{

  hide = true;


  AName = new FormControl("",[Validators.required,Validators.minLength(1),Validators.maxLength(100)]);
  AEmail = new FormControl("",[Validators.required, Validators.email]);
  AYear = new FormControl("",[Validators.required, Validators.minLength(4), Validators.maxLength(4)]);
  AMonth = new FormControl("",[Validators.required,Validators.minLength(1),Validators.maxLength(12)]);
  ADay = new FormControl("",[Validators.required,Validators.minLength(1),Validators.maxLength(31)]);
  APassword = new FormControl("",[Validators.required,Validators.minLength(8),Validators.maxLength(32)]);
  APasswordRepeat = new FormControl("",[Validators.required]);

  private MatchPassword(): boolean {
      const password = this.APassword.value as string;
      const passwordRepeat = this.APasswordRepeat.value as string;

      if (password == passwordRepeat) {
        return true;
      }
      else {return false}

  }


  constructor(private http: HttpClient, public toastController: ToastController) {

  }

  async createAccount() {
    try {
      var day = this.ADay;

      
      //const d = new Date(parseInt(this.AYear), (this.AMonth-1).valueOf(), this.ADay)



      const oberservable = this.http.post<any>('http://localhost:5000/api/order', {
        name: this.AName,
        email: this.AEmail,
        birthday: null,
        password: this.APassword,
      });
      const result = await firstValueFrom<any>(oberservable);
      const toast = await this.toastController.create({
        message: result.message,
        color: "succes",
        duration: 5000
      })
      toast.present();
    }
    catch (e){
      console.log(e)
      if (e instanceof HttpErrorResponse){
        const toast = await this.toastController.create({
          message: e.message,
          color: "succes",
          duration: 5000
        });
        toast.present();
      }

    }
  }

  ngOnInit() {
  }


}
