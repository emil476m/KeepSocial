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
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {TokenService} from "../services/token.service";
import {FollowerslistModal} from "./FollowersList/Followerslist.modal";
import {FollowingListModal} from "./followingList/followingList.modal";

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
                      <ion-buttons slot="end">
                        <ion-button class="btnEdit"  id="topButton" *ngIf="isSelf" (click)="edit()" >Edit</ion-button>
                        <ion-button class="btnFriend" id="topButton" *ngIf="!isSelf"><ion-icon name="person-add-outline"></ion-icon></ion-button>
                        <ion-button class="btnFollow" id="topButton" *ngIf="!isSelf" (click)="changeFollow()" [textContent]="btnFollow"></ion-button>
                      </ion-buttons>
                    </ion-item>
                    <ion-card-title align="left">{{profileName}}</ion-card-title>

                      <ion-label style="text-align: left">{{profileDescription}}</ion-label>

                    <ion-item>
                      <ion-label (click)="openFolowingList(this.profileId)">{{following}} Following</ion-label>
                      <ion-label (click)="openFolowersList(this.profileId)">{{followers}} Followers</ion-label>
                    </ion-item>
                  </ion-col>
                </ion-row>
            </ion-grid>

        <div *ngIf="isSelf">
          <ion-card *ngIf="token.getToken()">
            <ion-toolbar>
              <ion-buttons>
                <ion-avatar>
                  <ion-img [src]="avatarUrl"/>
                </ion-avatar>
                <ion-text style="padding-left: 10px;">{{profileName}}</ion-text>
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
        </div>
        <ion-infinite-scroll (ionInfinite)="loadMore()">
          <ion-card *ngFor="let post of profilePosts" (click)="gotopost(post.id)">
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
          <ion-img *ngIf="post.imgUrl !== undefined" [src]="post.imgUrl" style="margin-left: 25%; margin-right: 25%; "/>
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
              private modalcontroller: ModalController,
              public fb: FormBuilder,

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

  file = new FormControl(null);

  imgUrl?: string;
  imageChangedEvent: Event | undefined;
  eventchange: Event|null = null;

  post = new FormGroup(
    {
      text: this.textFC,
      imgurl: this.imageFC,
    }
  )

  img = this.fb.group(
    {
      image: [null as Blob | null],
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

  openFolowingList(id:number) {
    this.state.profileId = id;
    this.modalcontroller.create(
      {
        component: FollowingListModal,
      }).then(res => {
      res.present();
    })
  }

  openFolowersList(id: number) {
    this.state.profileId = id;
    this.modalcontroller.create(
      {
        component: FollowerslistModal,
      }).then(res => {
      res.present();
    })
  }
}
