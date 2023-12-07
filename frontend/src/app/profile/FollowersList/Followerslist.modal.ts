import {Component, OnInit} from "@angular/core";
import {ModalController, PopoverController, ToastController} from "@ionic/angular";
import {Globalstate} from "../../services/states/globalstate";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";

@Component({
  template:
    `
      <ion-header>
        <ion-toolbar>
          <ion-title mode="ios">Followers</ion-title>
          <ion-buttons slot="end">
            <ion-button (click)="dismissModal()">Close</ion-button>
          </ion-buttons>
        </ion-toolbar>
      </ion-header>
    `,
})
export class FollowerslistModal implements OnInit
{
  constructor(
    public modalController: ModalController,
    public state: Globalstate,
    public toast: ToastController,
    public http: HttpClient,
    public router: Router,
    )
  {

  }

  ngOnInit(): void {
  }

  dismissModal() {
  this.modalController.dismiss();
  }
}
