import {Component, OnInit} from "@angular/core";
import {ActivatedRoute, NavigationStart, Router} from "@angular/router";
import {firstValueFrom} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {PostModel} from "../models/PostModel";
import {environment} from "../../environments/environment.prod";
import {Globalstate} from "../services/states/globalstate";
import * as ago from "s-ago";
import {TokenService} from "../services/token.service";
import {Account} from "../accountInterface";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {AlertController, ModalController, PopoverController, ToastController} from "@ionic/angular";
import {CommentModel} from "../models/CommentModel";
import {EditCommentModal} from "./EditCommentModal/edit.comment.modal";
import {EditPostModal} from "./EditPostModal/edit.post.modal";

@Component({
    selector: 'post-detail',
    template: `
      <ion-content>
        <ion-card>
          <ion-toolbar>
            <ion-title>{{this.state.currentPost.authorName}}</ion-title>
            <ion-buttons slot="end">
              <ion-text>created {{getLocalDate(this.state.currentPost.created)}}</ion-text>
            </ion-buttons>
            <ion-buttons slot="end" *ngIf="userid == this.state.currentPost.authorId">
              <ion-button (click)="ismenueopenPost()">
                <ion-icon name="ellipsis-vertical-outline"></ion-icon>
              </ion-button>
              <ion-popover [isOpen]="isopenPostMenu">
                <ng-template>

                  <ion-button fill="clear" (click)="openEditPost()">
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
          <ion-img *ngIf="this.state.currentPost.imgUrl != undefined" [src]="this.state.currentPost.imgUrl"/>
          <ion-text>{{this.state.currentPost.text}}</ion-text>
        </ion-card>
        <ion-toolbar>
          <ion-title mode="ios">Comments</ion-title>
        </ion-toolbar>
        <ion-card *ngIf="token.getToken()">
          <ion-toolbar>
            <ion-img [src]="profilepic" style="height: 30px; width: 30px; border-radius: 360%;"/>
            <ion-text>{{displayName}}</ion-text>
          </ion-toolbar>
          <ion-textarea [counter]="true" [maxlength]="500" placeholder="what do you want your comment to say?"
                        [formControl]="textFC"></ion-textarea>
          <div>
            <ion-input placeholder="image url" [formControl]="imageFC"></ion-input>
          </div>
          <ion-buttons>
            <ion-button [disabled]="comment.invalid" (click)="createComment()">Comment</ion-button>
          </ion-buttons>

        </ion-card>
        <ion-infinite-scroll (ionInfinite)="loadMore()">
          <ion-card *ngFor="let comment of this.state.comments">
            <ion-toolbar>
              <ion-buttons slot="end">
                <ion-text>created {{getLocalDate(comment.created)}}</ion-text>
              </ion-buttons>
              <ion-text>{{comment.authorName}}</ion-text>
              <ion-buttons slot="end" *ngIf="userid == comment.authorId">
                <ion-button (click)="ismenueopenComment()">
                  <ion-icon name="ellipsis-vertical-outline"></ion-icon>
                </ion-button>
                <ion-popover [isOpen]="isopenCommentMenu">
                  <ng-template>

                    <ion-button fill="clear" (click)="openEditComment(comment)">
                      <ion-icon name="create-outline"></ion-icon>
                      edit
                    </ion-button>
                    <br>
                    <ion-button fill="clear" color="danger" (click)="deleteAlertComment(comment.id)">
                      <ion-icon name="trash-outline"></ion-icon>
                      delete
                    </ion-button>

                  </ng-template>
                </ion-popover>
              </ion-buttons>
            </ion-toolbar>
            <ion-img *ngIf="comment.imgUrl != undefined" [src]="comment.imgUrl"/>
            <ion-text>{{comment.text}}</ion-text>
          </ion-card>
        </ion-infinite-scroll>
      </ion-content>`,
})
export class PostDetail implements OnInit
{
    displayName: string = "";
    profilepic: string = "";
    userid: number = 0;
    isopenCommentMenu = false;
    isopenPostMenu = false;

    limitFC = new FormControl(10,[Validators.required])
    textFC = new FormControl("",[Validators.required, Validators.maxLength(500), Validators.minLength(3)]);
    imageFC = new FormControl(null);

    comment = new FormGroup(
        {
            text: this.textFC,
            imgurl: this.imageFC,
        }
    )


    constructor(public router: Router,
                public route: ActivatedRoute,
                public http: HttpClient,
                public state: Globalstate,
                public token: TokenService,
                public toast: ToastController,
                public modalcontroller: ModalController,
                private alertcontroller: AlertController,
                public popoverCtrl: PopoverController)
    {
        this.router.events.subscribe(event =>    {
            if(event instanceof NavigationStart) {
                this.userid = 0;
                this.whoAmI();
                this.getPost();
                this.comment.reset();
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
            this.profilepic = result.avatarUrl;
            this.userid = result.userId;
        }
    }


    ngOnInit(): void {
        this.getPost()
        this.whoAmI()
    }

    async getPost() {
        const id = (await firstValueFrom(this.route.paramMap)).get('id');
        const call = this.http.get<PostModel>(environment.baseURL+'post/'+ id)
        const result = await firstValueFrom<PostModel>(call);
        this.state.currentPost = result;
        this.loadComments();
    }

    getLocalDate(UTCString: string) {
        let date = new Date(UTCString);
        date.setHours(date.getHours()+1)
        return ago (date);
    }

    async createComment() {
        try {
            const call = this.http.post<CommentModel>(environment.baseURL + "comment/", this.comment.value, {params:{postId: this.state.currentPost.id}});
            const result = await firstValueFrom<CommentModel>(call);
            console.log(result)
            this.state.comments.unshift(result);
            this.comment.reset();
            (await this.toast.create(
                {
                    message: "commented",
                    color: "success",
                    duration: 2000,
                })).present()
        }
        catch (e)
        {
            (await this.toast.create(
                {
                    message: "failed to comment please try again",
                    color: "danger",
                    duration: 2000,
                }
            )).present();
        }
    }

    private async loadComments() {
        const call = this.http.get<CommentModel[]>(environment.baseURL + "getcomments", {params:{postId: this.state.currentPost.id}})
        const result = await firstValueFrom<CommentModel[]>(call);
        this.state.comments = result;
    }

  async loadMore() {
        const call = this.http.get<PostModel[]>(environment.baseURL + "getmorecomments", {params: {limit: this.limitFC.value!, offset: this.state.comments.length, postId: this.state.currentPost.id}})
        const result = await firstValueFrom<PostModel[]>(call);
        this.state.posts = this.state.posts.concat(result)
    }


  ismenueopenComment() {
    if(this.isopenCommentMenu == false)
    {
      this.isopenCommentMenu = true;
    }
    else
    {
      this.isopenCommentMenu = false;
    }
  }



  deleteAlertComment(id: number) {
    this.alertcontroller.create({
      message: "do you want to delete this comment?",
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
            const call = this.http.delete(environment.baseURL+'deletecomment', {params:{id: id}});
            const result = await firstValueFrom(call);
            this.state.comments = this.state.comments.filter(e => e.id != id);
            this.popoverCtrl.dismiss();
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
              message: 'Failed to delete comment',
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

  openEditComment(comment: CommentModel) {
      this.state.currentComment = comment;
    this.modalcontroller.create({
      component: EditCommentModal,
      componentProps: {
         copyOfComment: {...this.state.currentComment}
      }
    }).then(res => {
      res.present();
    })
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
              this.state.posts = this.state.posts.filter(e => e.id != this.state.currentPost.id);
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

  openEditPost(){
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
