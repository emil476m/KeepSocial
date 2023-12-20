import {Component, OnInit} from '@angular/core';
import {TokenService} from "../services/token.service";
import {NavigationStart, Router} from "@angular/router";
import {AlertController, ModalController, PopoverController, ToastController} from "@ionic/angular";
import {AccountService} from "../services/account.service";
import {firstValueFrom} from "rxjs";
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {SimpleUser} from "../accountInterface";
import {PostModel} from '../models/PostModel';
import {Globalstate} from "../services/states/globalstate";
import * as ago from "s-ago";
import {EditPostModal} from "../PostDetailed/EditPostModal/edit.post.modal";

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
                  <ion-buttons>
                      <ion-avatar>
                          <ion-img [src]="profilepic"/>
                      </ion-avatar>
                      <ion-text style="padding-left: 10px;">{{displayName}}</ion-text>
                  </ion-buttons>
              </ion-toolbar>
              <ion-textarea [counter]="true" [maxlength]="500" placeholder="what do you want your post to say?"
                            [formControl]="textFC"></ion-textarea>
              <div>
                  <ion-img
                          *ngIf="imgUrl && !imageChangedEvent"
                          [src]="imgUrl"
                  ></ion-img>
                  <ion-input
                          [formControl]="file"
                          [id]="img"
                          label="image"
                          type="file"
                          accept="image/png, image/jpeg"
                          (change)="saveEvent($event)"
                  ></ion-input>
              </div>
              <ion-buttons>
                  <ion-button [disabled]="post.invalid" (click)="createPost()">post</ion-button>
              </ion-buttons>
          </ion-card>
          <ion-infinite-scroll (ionInfinite)="loadMore()">
              <ion-card *ngFor="let post of state.posts" (click)="gotopost(post.id)">

                  <ion-toolbar>
                      <ion-buttons slot="end">
                          <ion-text>created {{getLocalDate(post.created)}}</ion-text>
                      </ion-buttons>
                      <ion-buttons>
                          <ion-avatar>
                              <ion-img [src]="post.avatarUrl"/>
                          </ion-avatar>
                          <ion-text style="padding-left: 10px;">{{post.authorName}}</ion-text>
                      </ion-buttons>
                  </ion-toolbar>
                  <ion-img *ngIf="post.imgUrl != undefined" [src]="post.imgUrl" style="margin-left: 25%; margin-right: 25%; "/>
                  <ion-text>{{post.text}}</ion-text>
              </ion-card>
          </ion-infinite-scroll>
      </ion-content>
  `,
  styleUrls: ['home.page.scss'],
})
export class HomePage implements OnInit{

  displayName: string = "";
  profilepic: string | undefined = "";
  userid: number = 0;
  eventchange: Event|null = null;

  textFC = new FormControl("",[Validators.required, Validators.maxLength(500), Validators.minLength(3)]);
  imgFC = new FormControl(this.imgUrl);
  limitFC = new FormControl(10,[Validators.required])
  file = new FormControl(null);

  imgUrl?: string;
  imageChangedEvent: Event | undefined;

  post = new FormGroup (
      {
        text: this.textFC,
        imgurl: this.imgFC,
      }
  )

  img = this.fb.group(
    {
      image: [null as Blob | null],
    }
  )




  constructor(public token: TokenService,
              private router : Router,
              private readonly toast: ToastController,
              private aService: AccountService,
              private http : HttpClient,
              public state: Globalstate,
              public modalcontroller: ModalController,
              private alertcontroller: AlertController,
              public popoverCtrl: PopoverController,
              public fb: FormBuilder)
  {
    this.router.events.subscribe(event =>    {
      if(event instanceof NavigationStart) {
        this.state.posts = [];
        this.userid = 0;
        this.whoAmI();
        this.loadPosts();
        this.post.reset();
        this.file.reset();
      }
    })
  }

  async whoAmI()
  {
    if(this.token.getToken())
    {
    const call = this.http.get<SimpleUser>(environment.baseURL+"account/simpleuser");
    const result = await firstValueFrom<SimpleUser>(call);
    this.displayName = result.userDisplayname;
    this.profilepic = result.avatarUrl;
    this.userid = result.userId;
    this.state.currentUserName = result.userDisplayname;
    }
  }


  async logout() {
    try{
    this.token.clearToken();
    this.userid = 0;
    this.state.currentUserName = "";

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
      if (this.eventchange) {
       await  this.uploadimg(this.eventchange);
      }
      this.post.patchValue({imgurl: this.imgUrl});
      const call = this.http.post<PostModel>(environment.baseURL + "post", this.post.value);
      const result = await firstValueFrom<PostModel>(call);
      this.state.posts.unshift(result);
      this.post.reset();
      this.img.reset();
      this.file.reset();

      this.imgUrl = undefined;
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
    const call = this.http.get<PostModel[]>(environment.baseURL + "getposts", {params: {limit: this.limitFC.value!, offset: this.state.posts.length}})
    const result = await firstValueFrom<PostModel[]>(call);
    this.state.posts = this.state.posts.concat(result)
  }

  private async loadPosts() {
    const call = this.http.get<PostModel[]>(environment.baseURL + "getposts",{params: {limit: this.limitFC.value!, offset: 0}})
    const result = await firstValueFrom<PostModel[]>(call);
    this.state.posts = result;
  }

  getLocalDate(UTCString: string) {
let date = new Date(UTCString);
    date.setHours(date.getHours()-1)
    return ago (date);
  }

  gotopost(id: number) {
    this.router.navigate(['post/'+id]);
  }

  async uploadimg($event: Event) {
    // The event contains all/any selected files
    const files = ($event.target as HTMLInputElement).files;
    if (!files) return;
    const file = files[0];
    const formData = new FormData();
    formData.set("image", file);
    const res = await firstValueFrom<any>(this.http.post(environment.baseURL + 'commentimg?url='+this.imgUrl, formData ));
    this.imgUrl = res.messageToClient
  }

  saveEvent($event: Event) {
    this.eventchange = $event;
  }
}


