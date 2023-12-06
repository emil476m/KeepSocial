import {Component, OnInit} from "@angular/core";
import {
  AlertController,
  ModalController,
  PopoverController,
  ToastController
} from "@ionic/angular";
import {firstValueFrom} from "rxjs";
import {ActivatedRoute, NavigationStart, Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {Profile} from "../accountInterface";
import {environment} from "../../environments/environment.prod";
import {Globalstate} from "../services/states/globalstate";
import {NewAccountInfoModal} from "../changeAccountInfoModal/AccountInfoModal";
import {PostModel} from "../models/PostModel";
import * as ago from "s-ago";
import {EditPostModal} from "../PostDetailed/EditPostModal/edit.post.modal";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {TokenService} from "../services/token.service";

@Component({
  styleUrls: ['profile.page.style.css'],
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
                      <ion-img style="width: 300px; height: 300px; text-align: left;" [src]="avatarUrl"></ion-img>
                  </ion-col>
                  <ion-col size="2" >
                  </ion-col>

                  <ion-col size="9" style="vertical-align: text-bottom" >
                    <ion-item lines="none">
                      <ion-buttons slot="end" >
                        <ion-button class="btnEdit"  *ngIf="isSelf" (click)="edit()" >Edit</ion-button>
                        <ion-button class="btnFriend" *ngIf="!isSelf"><ion-icon name="person-add-outline"></ion-icon></ion-button>
                        <ion-button class="btnFollow" *ngIf="!isSelf" (click)="changeFollow()" [textContent]="btnFollow"></ion-button>
                      </ion-buttons>
                    </ion-item>
                    <ion-card-title align="left">{{profileName}}</ion-card-title>

                      <ion-label style="text-align: left">{{profileDescription}}</ion-label>

                    <ion-item>
                      <ion-label>{{following}} Following</ion-label>
                      <ion-label>{{followers}} Followers</ion-label>
                    </ion-item>
                  </ion-col>
                </ion-row>
            </ion-grid>

        <div *ngIf="isSelf">
        <ion-card *ngIf="token.getToken()">
          <ion-toolbar>
            <ion-img [src]="avatarUrl" style="height: 30px; width: 30px; border-radius: 360%;"/>
            <ion-text>{{profileName}}</ion-text>
          </ion-toolbar>
          <ion-textarea [counter]="true" [maxlength]="500" placeholder="what do you want your post to say?" [formControl]="textFC"></ion-textarea>
          <div>
            <ion-input placeholder="image url" [formControl]="imageFC"></ion-input>
          </div>
          <ion-buttons>
            <ion-button [disabled]="post.invalid" (click)="createPost()">post</ion-button>
          </ion-buttons>
        </ion-card>
        </div>
        <ion-infinite-scroll (ionInfinite)="loadMore()">
          <ion-card *ngFor="let post of profilePosts" (click)="gotopost(post.id)">

            <ion-toolbar><ion-buttons slot="end">
              <ion-text >created {{getLocalDate(post.created)}}</ion-text>
            </ion-buttons>
              <ion-text>{{post.authorName}}</ion-text>
              <ion-buttons slot="end" *ngIf="isSelf">
                <ion-button (click)="ismenueopenPost()">
                  <ion-icon name="ellipsis-vertical-outline"></ion-icon>
                </ion-button>
                <ion-popover [isOpen]="isopenPostMenu">
                  <ng-template>

                    <ion-button fill="clear" (click)="openEditPost(post)">
                      <ion-icon name="create-outline"></ion-icon>
                      edit
                    </ion-button>
                    <br>
                    <ion-button fill="clear" color="danger" (click)="DeleteAlertPost()">
                      <ion-icon name="trash-outline"></ion-icon>
                      delete
                    </ion-button>

                  </ng-template>
                </ion-popover>
              </ion-buttons>
            </ion-toolbar>
            <ion-img *ngIf="post.imgUrl != undefined" [src]="post.imgUrl"/>
            <ion-text>{{post.text}}</ion-text>
          </ion-card>
        </ion-infinite-scroll>


      </ion-content>


        `,
})

export class ProfilePage implements OnInit{

  constructor(public router: Router,
              private alertcontroller: AlertController,
              public popoverCtrl: PopoverController,
              public toast: ToastController,
              public token: TokenService,
              public route: ActivatedRoute,
              public http: HttpClient,
              public state: Globalstate,
              private modalcontroller: ModalController

  ){
    this.router.events.subscribe(event =>    {
      if(event instanceof NavigationStart) {
        this.getProfileData();
      }
    })
  }


  defaultAvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png";

  profileId = 0;
  profilePosts : PostModel[] = [];
  followers = 0;
  following = 0;
  profileName = "";
  profileDescription = "";
  avatarUrl = "";
  postAmount = 0;
  isFriend = false;
  isSelf = false;
  isFollowing = false;

  btnFriend = ""
  btnFollow = "Follow";

  isopenPostMenu = false;

  textFC = new FormControl("",[Validators.required, Validators.maxLength(500), Validators.minLength(3)]);
  imageFC = new FormControl(null);
  limitFC = new FormControl(10,[Validators.required])

  post = new FormGroup(
    {
      text: this.textFC,
      imgurl: this.imageFC,
    }
  )

  ngOnInit() {
    this.state.updatingWhatAccountItem=null;
    this.getProfileData();


  }

  async getProfileData(){
    this.profileName = <string>(await firstValueFrom(this.route.paramMap)).get('profileName');

    const call = this.http.get<Profile>(environment.baseURL+"getProfile"+"/"+this.profileName);
    const result = await firstValueFrom<Profile>(call);
    this.profileId = result.userId;
    this.followers = result.followers;
    this.following = result.following;
    this.avatarUrl = result.avatarUrl;
    if (this.avatarUrl == "" || this.avatarUrl == null){
      this.avatarUrl = this.defaultAvatarUrl;
    }
    this.profileDescription = result.profileDescription;
    this.postAmount = result.postAmount;
    this.isFriend = result.isFriend;
    this.isSelf = result.isSelf;
    this.isFollowing = result.isFollowing;
    if (this.isFollowing){
      this.btnFollow = "UnFollow";
    }
    this.getProfilePosts();

  }


  changeFollow(){
    if (!this.isSelf && this.isFollowing) {
      this.unFollow()
      this.isFollowing = false;
      this.btnFollow = "Follow";
    }
    else if (!this.isSelf && !this.isFollowing){
      this.follow()
      this.isFollowing = true;
      this.btnFollow = "UnFollow";
    }
  }
  async unFollow(){
    const call = this.http.delete<number>(environment.baseURL+"account/unFollowUser/"+this.profileId);
    const result = await firstValueFrom<number>(call);

  }
  async follow(){
    const call = this.http.post(environment.baseURL+"account/FollowUser",this.profileId);
    const result = await firstValueFrom(call);

  }

  edit(){
    this.state.updatingWhatAccountItem="Profile Description";
    this.openEdit();
  }

  openEdit() {

    this.modalcontroller.create({
      component: NewAccountInfoModal,
      componentProps: {
      }
    }).then(res => {
      res.present();
    })
  }

  async getProfilePosts() {

    const call = this.http.get<PostModel[]>(environment.baseURL +'userprofilepost/'+ this.profileId)
    const result = await firstValueFrom<PostModel[]>(call);
    this.profilePosts = result

  }

  async createPost() {
    try {
      const call = this.http.post<PostModel>(environment.baseURL + "post", this.post.value);
      const result = await firstValueFrom<PostModel>(call);
      this.profilePosts.unshift(result);
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
    const call = this.http.get<PostModel[]>(environment.baseURL + "getmoreprofileposts/"+this.profileId, {params: {limit: this.limitFC.value!, offset: this.profilePosts.length}})
    const result = await firstValueFrom<PostModel[]>(call);
    this.profilePosts = this.profilePosts.concat(result)
  }

  getLocalDate(UTCString: string) {
    let date = new Date(UTCString);
    date.setHours(date.getHours()+1)
    return ago (date);
  }

  gotopost(id: number) {
    this.router.navigate(['post/'+id]);
  }

  ismenueopenPost() {
    if(this.isopenPostMenu == false)
    {
      this.isopenPostMenu = true;
    }
    else {
      this.isopenPostMenu = false;
    }
  }

  DeleteAlertPost() {
    this.alertcontroller.create({
      message: "do you want to delete this post?",
      buttons: [
        {
          role: "cancel",
          text: "no"
        },
        {
          role: "confirm",
          text: "yes",
          handler: async () => {
            try{
              const call = this.http.delete(environment.baseURL+'deletepost', {params:{id: this.state.currentPost.id}});
              const result = await firstValueFrom(call);
              this.profilePosts = this.profilePosts.filter(e => e.id != this.state.currentPost.id);
              this.popoverCtrl.dismiss();
              this.router.navigate(['home']);
              this.toast.create({
                color: "success",
                message: ' successfully deleted.',
                duration: 2000,
              }).then(res =>
              {
                res.present();
              })

            }
            catch (e)
            {
              ((await this.toast.create({
                message: 'Failed to delete post',
                color: "danger",
                duration: 2000,
              }))).present
            }}
        }
      ]}).then(res =>
    {
      res.present();
    })
  }


  openEditPost(post: PostModel){
    this.state.currentPost = post;
    this.modalcontroller.create({
      component: EditPostModal,
      componentProps: {
        copyOfPost: {...this.state.currentPost}
      }
    }).then(res => {
      res.present();
    })
  }

}
