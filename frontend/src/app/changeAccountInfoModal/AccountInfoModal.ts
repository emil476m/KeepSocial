import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ModalController, ToastController} from "@ionic/angular";
import {FormControl, Validators} from "@angular/forms";
import {firstValueFrom} from "rxjs";
import {Globalstate} from "../services/states/globalstate";

@Component({
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
        <ion-item>
          <ion-input [formControl]="UserInput" type="text" onpaste="return false;" ondrop="return false;" autocomplete="off" label-placement="floating" [label]="InputLabel"></ion-input>
          <div *ngIf="UserInput.invalid && UserInput.touched" class="error"  [textContent]="'Invalid '+this.globalstate.updatingWhatAccountItem"> Invalid
          </div>
        </ion-item>
        <ion-button [disabled]="UserInput.invalid" (click)="updateAccount()">Confirm</ion-button>
      </ion-content>
    `,
})
export class NewAccountInfoModal implements OnInit
{

  UserInput : FormControl= new FormControl("");
  InputLabel = this.globalstate.updatingWhatAccountItem;


  ngOnInit(): void {
    this.setup()
  }

  setup(){
    if (this.globalstate.updatingWhatAccountItem == "Account Name"){
      this.UserInput.setValidators([Validators.required, Validators.minLength(1),Validators.maxLength(100)])

    }else if (this.globalstate.updatingWhatAccountItem == "Account Email"){
      this.UserInput.setValidators([Validators.required, Validators.email])

    }else if (this.globalstate.updatingWhatAccountItem == "Account Password"){

    }else if (this.globalstate.updatingWhatAccountItem == "Account Avatar"){

    }
  }


  constructor(private modalController: ModalController, private http: HttpClient, private toastControl: ToastController, public globalstate: Globalstate) {
  }

  dismissModal() {
    this.globalstate.updatingWhatAccountItem=null;
    this.modalController.dismiss();
  }


  async updateAccount() {
    try {
      const oberservable = this.http.post<string>('http://localhost:5000/api/account/updateAccount', {
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
