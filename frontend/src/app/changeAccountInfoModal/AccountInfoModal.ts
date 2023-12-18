import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ModalController, ToastController} from "@ionic/angular";
import {FormControl, Validators} from "@angular/forms";
import {firstValueFrom} from "rxjs";
import {Globalstate} from "../services/states/globalstate";
import {BoolResponse} from "../accountInterface";
import {environment} from "../../environments/environment.prod";

@Component({
  styleUrls: ["AccountInfoModal.style.css"],
  template:
    `
      <ion-header>
        <ion-toolbar>
          <ion-title>Change Account Info</ion-title>
          <ion-buttons slot="end">
            <ion-button (click)="dismissModal()">Close</ion-button>
          </ion-buttons>
        </ion-toolbar>
      </ion-header>
      <ion-content>
        <ion-item *ngIf="IsValidated">
          <ion-input [formControl]="UserInput" type="text" label-placement="floating" [label]="InputLabel" [type]="hide ? 'password' : 'text'"></ion-input>
          <div *ngIf="UserInput.invalid && UserInput.touched" class="error"  [textContent]="'Invalid '+this.globalstate.updatingWhatAccountItem"> Invalid
          </div>
        </ion-item>
        <ion-button *ngIf="IsValidated" [disabled]="UserInput.invalid" (click)="updateAccount()">Confirm</ion-button>
          <ion-item *ngIf="!IsValidated">
              <ion-label>Please validate enter validation code from email</ion-label>
              <ion-input class="numberInput" [formControl]="ValidationCode" type="number" label-placement="floating" label="ValidationCode"></ion-input>
          </ion-item>
          <ion-item *ngIf="!IsValidated">
              <ion-button (click)="sendCode()">Send Code</ion-button>
              <ion-button *ngIf="IsCodeSent" [disabled]="!is8long()" (click)="validateCode()">Confirm Code</ion-button>
              <div *ngIf="!is8long() && ValidationCode.touched">must be 8 characters long</div>
          </ion-item>
      </ion-content>
    `,
})

export class NewAccountInfoModal implements OnInit
{

  UserInput : FormControl= new FormControl("");
  InputLabel = this.globalstate.updatingWhatAccountItem;
  IsValidated : boolean = false;

  IsCodeSent = false;

  hide = false;
  ValidationCode: FormControl = new FormControl(Number, []);


  ngOnInit(): void {
    this.setup()
  }

  setup(){
    if (this.globalstate.updatingWhatAccountItem == "Account Name"){
      this.UserInput.setValidators([Validators.required, Validators.minLength(1),Validators.maxLength(100)])
      this.IsValidated = true;

    }else if (this.globalstate.updatingWhatAccountItem == "Account Email"){
      this.UserInput.setValidators([Validators.required, Validators.email])

    }else if (this.globalstate.updatingWhatAccountItem == "Account Password"){
      this.UserInput.setValidators([Validators.required, Validators.minLength(8),Validators.maxLength(32)])
      this.hide=true;
    }else if (this.globalstate.updatingWhatAccountItem == "Account Avatar"){
      this.UserInput.setValidators([Validators.required])
      this.IsValidated = true;
    }
    else if (this.globalstate.updatingWhatAccountItem == "Profile Description"){
      this.UserInput.setValidators([Validators.required, Validators.maxLength(500)])
      this.IsValidated = true;
    }
  }


  constructor(private modalController: ModalController, private http: HttpClient, private toastControl: ToastController, public globalstate: Globalstate) {
  }

  is8long()  {
    if (!this.IsCodeSent) return false
    if (this.ValidationCode.value.toString().length == 8){
      return true;
    }
    else return false
  }

  dismissModal() {
    this.globalstate.updatingWhatAccountItem=null;
    this.modalController.dismiss();
  }

  async sendCode(){
    const call = this.http.post(environment.baseURL+"account/validationGeneration", {});
    const result = await firstValueFrom(call);
    this.IsCodeSent = true;
  }

  async validateCode(){
    const call = this.http.post<BoolResponse>(environment.baseURL+"account/validationConfirmation",
      this.ValidationCode.value
    );
    const result = await firstValueFrom<BoolResponse>(call);
    this.IsValidated = result.isTrue;


  }


  async updateAccount() {
    if (!this.IsValidated){return}
    try {
      const oberservable = this.http.post<string>(environment.baseURL+'account/updateAccount', {
        updatedValue: this.UserInput.value,
        updatedValueName: this.globalstate.updatingWhatAccountItem
      });
      const result = await firstValueFrom<string>(oberservable);
      window.location.reload();
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
          message: "failed to update account"
        }
      ).then(res =>{
        res.present();
      })
    }
    this.dismissModal();
  }

}
