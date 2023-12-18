import {Component, OnInit} from "@angular/core";
import {ModalController, ToastController} from "@ionic/angular";
import {Globalstate} from "../../services/states/globalstate";
import {HttpClient} from "@angular/common/http";
import {NavigationStart, Router} from "@angular/router";
import {SimpleUser} from "../../accountInterface";
import {environment} from "../../../environments/environment";
import {firstValueFrom} from "rxjs";
import {FormControl, Validators} from "@angular/forms";

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
      <ion-content>
        <ion-infinite-scroll (ionInfinite)="loadMore()">
          <ion-card *ngFor="let user of Followers" (click)="gotoProfile(user.userDisplayname)">
            <ion-toolbar>
              <ion-buttons>
                <ion-avatar>
                  <ion-img [src]="user.avatarUrl"/>
                </ion-avatar>
                <ion-text style="padding-left: 10px">{{user.userDisplayname}}</ion-text>
              </ion-buttons>

            </ion-toolbar>
          </ion-card>
        </ion-infinite-scroll>
      </ion-content>
    `,
})
export class FollowerslistModal implements OnInit
{
  Followers : SimpleUser[] = []
  limitFC = new FormControl(10,[Validators.required])
  constructor(
    public modalController: ModalController,
    public state: Globalstate,
    public toast: ToastController,
    public http: HttpClient,
    public router: Router,
    )
  {
    this.router.events.subscribe(event =>    {
      if(event instanceof NavigationStart) {
       this.getFollowers();
      }
    })
  }

  ngOnInit(): void {
    this.getFollowers();
  }

  dismissModal() {
  this.modalController.dismiss();
  }

  async getFollowers() {
    const call = this.http.get<SimpleUser[]>(environment.baseURL+"account/getfollowers", {
      params: {
        id: this.state.profileId!,
        offset: 0,
        limit: this.limitFC.value!,
      }
    });
    const result = await firstValueFrom<SimpleUser[]>(call);
    this.Followers = result;
  }

  async loadMore() {
    const call = this.http.get<SimpleUser[]>(environment.baseURL+"account/getfollowers", {
      params: {
        id: this.state.profileId!,
        offset: this.Followers.length,
        limit: this.limitFC.value!,
      }
    });
    const result = await firstValueFrom<SimpleUser[]>(call);
    this.Followers.concat(result);
  }

  gotoProfile(displayname: string)
  {
    this.modalController.dismiss();
    this.router.navigate(['profile/'+displayname])
  }
}
