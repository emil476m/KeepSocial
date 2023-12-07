import {Component, OnInit} from "@angular/core";
import {Profile} from "../accountInterface";
import {environment} from "../../environments/environment.prod";
import {firstValueFrom} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {FormControl, Validators} from "@angular/forms";
import {Router} from "@angular/router";

@Component({
  template:
    `
      <ion-content>
        <ion-header>
          <ion-card-header align="center">Search</ion-card-header>
          <ion-item>
            <ion-input [formControl]="searchTerm" align="center">Serach For Users</ion-input>
            <ion-button (click)="getSearchResults()">Search</ion-button>
          </ion-item>

        </ion-header>


        <ion-infinite-scroll (ionInfinite)="getMoreSearchResults()">
          <ion-card *ngFor="let profile of searchResults" (click)="goToProfile(profile.userDisplayName)">

            <ion-toolbar><ion-buttons slot="end">
            </ion-buttons>

            </ion-toolbar>
            <ion-img *ngIf="profile.avatarUrl != undefined" [src]="profile.avatarUrl"/>
            <ion-text>{{profile.userDisplayName}}</ion-text>
          </ion-card>
        </ion-infinite-scroll>

      </ion-content>
        `,
})

export class SearchPage implements OnInit{

  searchTerm = new FormControl("",[Validators.minLength(3)]);
  limitFC = new FormControl(10,[Validators.required]);
  searchResults : Profile[] = [];
  constructor(public http: HttpClient,
              public router: Router) {
  }

  ngOnInit() {
  }


  async getSearchResults(){
    this.router.navigate(['search/'+this.searchTerm.value]);
    const call = this.http.get<Profile>(environment.baseURL+"search/"+ this.searchTerm.value, {params: {limit: this.limitFC.value!, offset: this.searchResults.length}});
    const result = await firstValueFrom<Profile>(call);
    this.searchResults = this.searchResults.concat(result);
  }

  async getMoreSearchResults(){
    const call = this.http.get<Profile>(environment.baseURL+"search/"+ this.searchTerm.value, {params: {limit: this.limitFC.value!, offset: this.searchResults.length}});
    const result = await firstValueFrom<Profile>(call);
    this.searchResults = this.searchResults.concat(result);
  }

  goToProfile(profileName : string){
    this.router.navigate(['profile/'+profileName]);
  }
}
