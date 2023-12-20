import {Component, OnInit} from "@angular/core";
import {Profile, SimpleUser} from "../accountInterface";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {FormControl, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  template:
    `
      <ion-content>
        <ion-header>
          <ion-card-header align="center">Search</ion-card-header>
          <ion-item>
            <ion-searchbar placeholder="Search for Users" [formControl]="searchTerm" align="center"></ion-searchbar>
            <ion-button (click)="changeRoute()">Search</ion-button>
          </ion-item>
        </ion-header>

        <ion-infinite-scroll (ionInfinite)="getMoreSearchResults()">
          <ion-card *ngFor="let profile of searchResults" (click)="goToProfile(profile.userDisplayname)">

            <ion-toolbar><ion-buttons slot="end">
            </ion-buttons>

            </ion-toolbar>
              <ion-avatar style="margin-left: 3%">
                  <ion-img *ngIf="profile.avatarUrl != undefined" [src]="profile.avatarUrl"/>
                  <ion-img *ngIf="profile.avatarUrl == undefined" [src]="defaultAvatarUrl"></ion-img>
              </ion-avatar>

            <ion-text style="margin-left: 3%">{{profile.userDisplayname}}</ion-text>
          </ion-card>
        </ion-infinite-scroll>

      </ion-content>
        `,
})

export class SearchPage implements OnInit{

  defaultAvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png";

  searchTerm = new FormControl("",[Validators.minLength(3)]);
  limitFC = new FormControl(10,[Validators.required]);
  searchResults : SimpleUser[] = [];
  constructor(public http: HttpClient,
              public router: Router,
              public route: ActivatedRoute) {
  }

  ngOnInit() {
    this.getSearchResults();
  }


  changeRoute(){
    this.router.navigate(['search/'+this.searchTerm.value]);
  }

  async getSearchResults(){

    var routeSearchTerm = <string>(await firstValueFrom(this.route.paramMap)).get('searchTerm');
    const call = this.http.get<SimpleUser>(environment.baseURL+"search/"+ routeSearchTerm, {params: {limit: this.limitFC.value!, offset: this.searchResults.length}});
    const result = await firstValueFrom<SimpleUser>(call);
    this.searchResults = this.searchResults.concat(result);
  }

  async getMoreSearchResults(){
    var routeSearchTerm = <string>(await firstValueFrom(this.route.paramMap)).get('searchTerm');
    const call = this.http.get<SimpleUser>(environment.baseURL+"search/"+ routeSearchTerm, {params: {limit: this.limitFC.value!, offset: this.searchResults.length}});
    const result = await firstValueFrom<SimpleUser>(call);
    this.searchResults = this.searchResults.concat(result);
  }

  goToProfile(profileName : string){
    this.router.navigate(['profile/'+profileName]);
  }
}
