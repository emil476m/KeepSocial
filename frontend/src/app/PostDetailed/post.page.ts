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
import {ToastController} from "@ionic/angular";
import {CommentModel} from "../models/CommentModel";

@Component({
    selector: 'post-detail',
    template: `<ion-content>
    <ion-card>
        <ion-toolbar>
            <ion-title>{{this.state.currentPost.name}}</ion-title>
            <ion-buttons slot="end">
                <ion-text >created {{getLocalDate(this.state.currentPost.created)}}</ion-text>
            </ion-buttons>
        </ion-toolbar>
        <ion-img *ngIf="this.state.currentPost.img_url != undefined" [src]="this.state.currentPost.img_url"/>
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
        <ion-textarea [counter]="true" [maxlength]="500" placeholder="what do you want your comment to say?" [formControl]="textFC"></ion-textarea>
        <div>
            <ion-input placeholder="image url" [formControl]="imageFC"></ion-input>
        </div>
        <ion-buttons>
            <ion-button [disabled]="comment.invalid" (click)="createComment()">Comment</ion-button>
        </ion-buttons>
    </ion-card>
        <ion-infinite-scroll (ionInfinite)="loadMore()">
            <ion-card *ngFor="let comment of this.state.comments">
                <ion-toolbar><ion-buttons slot="end">
                    <ion-text >created {{getLocalDate(comment.created)}}</ion-text>
                </ion-buttons>
                    <ion-text>{{comment.name}}</ion-text>

                </ion-toolbar>
                <ion-img *ngIf="comment.img_url != undefined" [src]="comment.img_url"/>
                <ion-text>{{comment.text}}</ion-text>
            </ion-card>
        </ion-infinite-scroll>
    </ion-content>`,
})
export class PostDetail implements OnInit
{
    displayName: string = "";
    profilepic: string = "";

    limitFC = new FormControl(10,[Validators.required])
    textFC = new FormControl("",[Validators.required, Validators.maxLength(500), Validators.minLength(3)]);
    imageFC = new FormControl(null);

    comment = new FormGroup(
        {
            text: this.textFC,
            imgurl: this.imageFC,
        }
    )

    constructor(public router: Router, public route: ActivatedRoute, public http: HttpClient, public state: Globalstate, public token: TokenService, public toast: ToastController)
    {
        this.router.events.subscribe(event =>    {
            if(event instanceof NavigationStart) {
                this.whoAmI();
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
            this.profilepic = result.AvatarUrl;
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
}
