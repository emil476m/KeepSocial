import {Component, OnInit} from '@angular/core';
import {TokenService} from "../services/token.service";
import {NavigationStart, Router} from "@angular/router";
import {ToastController} from "@ionic/angular";
import {AccountService} from "../services/account.service";
import {firstValueFrom} from "rxjs";
import {environment} from "../../environments/environment.prod";
import {HttpClient} from "@angular/common/http";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {Account} from "../accountInterface";
import {PostModel} from '../models/PostModel';
import {Globalstate} from "../services/states/globalstate";
import * as ago from "s-ago";

@Component({
  selector: 'app-home',
  template: `
    <ion-toolbar>
      <ion-title mode="ios">KeepSocial</ion-title>
      <ion-buttons slot="end">
          <ng-template #notLoggedin>
          <ion-button routerLink="/login">
              <ion-icon name="log-in-outline"></ion-icon>
              Login
          </ion-button>
        <ion-button routerLink="/register">Sign-up</ion-button>
          </ng-template>
        <ion-button (click)="logout()" *ngIf="token.getToken(); else notLoggedin">
          <ion-icon name="log-out-outline"></ion-icon>
          logout
        </ion-button>
      </ion-buttons>
    </ion-toolbar>
    <ion-content>
      <ion-card *ngIf="token.getToken()">
        <ion-toolbar>
          <ion-img [src]="profilepic" style="height: 30px; width: 30px; border-radius: 360%;"/>
          <ion-text>{{displayName}}</ion-text>
        </ion-toolbar>
          <ion-textarea [counter]="true" [maxlength]="500" placeholder="what do you want your post to say?" [formControl]="textFC"></ion-textarea>
          <div>
              <ion-input placeholder="image url" [formControl]="imageFC"></ion-input>
          </div>
          <ion-buttons>
              <ion-button [disabled]="post.invalid" (click)="createPost()">post</ion-button>
          </ion-buttons>
      </ion-card>
      <ion-infinite-scroll (ionInfinite)="loadMore()">
        <ion-card *ngFor="let post of state.posts" (click)="gotopost(post.id)">

          <ion-toolbar><ion-buttons slot="end">
            <ion-text >created {{getLocalDate(post.created)}}</ion-text>
          </ion-buttons>
            <ion-text>{{post.authorName}}</ion-text>

          </ion-toolbar>
          <ion-img *ngIf="post.imgUrl != undefined" [src]="post.imgUrl"/>
          <ion-text>{{post.text}}</ion-text>
        </ion-card>
      </ion-infinite-scroll>
    </ion-content>
  `,
  styleUrls: ['home.page.scss'],
})
export class HomePage implements OnInit{

  displayName: string = "";
  profilepic: string = "";
  textFC = new FormControl("",[Validators.required, Validators.maxLength(500), Validators.minLength(3)]);
  imageFC = new FormControl(null);
  limitFC = new FormControl(10,[Validators.required])

  post = new FormGroup(
      {
        text: this.textFC,
        imgurl: this.imageFC,
      }
  )




  constructor(public token: TokenService, private router : Router, private readonly toast: ToastController, private aService: AccountService, private http : HttpClient, public state: Globalstate)
  {
    this.router.events.subscribe(event =>    {
      if(event instanceof NavigationStart) {
        this.whoAmI();
      }
    })
  }

  async whoAmI()
  {
    if(this.token.getToken())
    {
    const call = this.http.get<Account>(environment.baseURL+"whoami");
    const result = await firstValueFrom<Account>(call);
    this.displayName = result.userDisplayName;
    this.profilepic = result.AvatarUrl;
    }
  }


  async logout() {
    try{
    this.token.clearToken();

    (await this.toast.create({
      message: "Logout successful",
      duration: 5000,
      color: 'success',
    })).present()
    }
    catch (e)
    {
      (await this.toast.create(
        {
          message: "failed to logout",
          color: "danger",
          duration: 5000,
        }
      )).present();
    }
  }

  ngOnInit(): void
  {
    this.whoAmI();
    this.loadPosts();
  }

  async createPost() {
    try {
      const call = this.http.post<PostModel>(environment.baseURL + "post", this.post.value);
      const result = await firstValueFrom<PostModel>(call);
      this.state.posts.unshift(result);
      this.post.reset();
      (await this.toast.create(
          {
            message: "Posted",
            color: "success",
            duration: 2000,
          })).present()
    }
    catch (e)
    {
      (await this.toast.create(
          {
            message: "failed to post please try again",
            color: "danger",
            duration: 2000,
          }
      )).present();
    }
  }

  async loadMore() {
    const call = this.http.get<PostModel[]>(environment.baseURL + "getmoreposts", {params: {limit: this.limitFC.value!, offset: this.state.posts.length}})
    const result = await firstValueFrom<PostModel[]>(call);
    this.state.posts = this.state.posts.concat(result)
  }

  private async loadPosts() {
    const call = this.http.get<PostModel[]>(environment.baseURL + "getposts",)
    const result = await firstValueFrom<PostModel[]>(call);
    this.state.posts = result;
  }

  getLocalDate(UTCString: string) {
let date = new Date(UTCString);
date.setHours(date.getHours()+1)
    return ago (date);
  }

  gotopost(id: number) {
    this.router.navigate(['post/'+id]);
  }
}


