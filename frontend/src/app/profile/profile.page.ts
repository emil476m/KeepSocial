import {Component, OnInit} from "@angular/core";
import {InfiniteScrollCustomEvent} from "@ionic/angular";
import {firstValueFrom} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";

@Component({
  template:
    `
      <ion-content>

        <ion-card-header align="center">
          <ion-card-title>{{profileName}}</ion-card-title>
          <ion-card-subtitle>{{postAmount}} posts</ion-card-subtitle>
        </ion-card-header>

            <ion-grid>
                <ion-row>
                  <ion-col size="1" >
                      <ion-img style="width: 300px; height: 300px; text-align: left;" [src]="defaultAvatarUrl"></ion-img>
                  </ion-col>
                  <ion-col size="2" >
                  </ion-col>

                  <ion-col size="9" style="text-align: right; vertical-align: text-bottom" >
                    <ion-button><ion-icon name="person-add-outline"></ion-icon></ion-button>
                    <ion-button>Follow</ion-button>
                    <ion-card-title align="left">{{profileName}}</ion-card-title>
                    <ion-label style="text-align: left">{{profileDescription}}</ion-label>
                    <ion-item>
                      <ion-label>{{following}} Following</ion-label>
                      <ion-label>{{followers}} Followers</ion-label>
                    </ion-item>
                  </ion-col>
                </ion-row>
            </ion-grid>
        <ion-list>
          <ion-item *ngFor="let item of items; let index">
            <ion-label>{{ item }}</ion-label>
          </ion-item>
        </ion-list>
        <ion-infinite-scroll>
          <ion-infinite-scroll-content loadingText="Please wait..." loadingSpinner="bubbles"></ion-infinite-scroll-content>
        </ion-infinite-scroll>
      </ion-content>
        `,
})

export class ProfilePage implements OnInit{

  constructor(public router: Router, public route: ActivatedRoute, public http: HttpClient){

  }
  defaultAvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png";

  items = [];
  followers = 0;
  following = 0;
  profileName = "";
  profileDescription = "";
  postAmount = 0;
  isFriend = false;

  ngOnInit() {
    for (let i = 1; i < 51; i++) {
      // @ts-ignore
      this.items.push(`Item ${i}`);
    }
    this.getProfileData();

  }

  async getProfileData(){
    this.profileName = <string>(await firstValueFrom(this.route.paramMap)).get('profileName');

  }

}
