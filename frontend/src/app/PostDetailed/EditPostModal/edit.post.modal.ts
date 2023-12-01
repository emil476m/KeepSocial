import {Component, OnInit} from "@angular/core";
import {ModalController, PopoverController, ToastController} from "@ionic/angular";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {CommentModel} from "../../models/CommentModel";
import {Globalstate} from "../../services/states/globalstate";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment.prod";
import {HttpClient} from "@angular/common/http";
import {NavigationStart, Router} from "@angular/router";
import {PostModel} from "../../models/PostModel";

@Component({
  template:
    `
      <ion-header>
        <ion-toolbar>
          <ion-title mode="ios">Post comment</ion-title>
          <ion-buttons slot="end">
            <ion-button (click)="dismissModal()">Close</ion-button>
          </ion-buttons>
        </ion-toolbar>
      </ion-header>
      <ion-content>
        <ion-textarea label="comment" labelPlacement="floating" [formControl]="textFC" [counter]="true" [maxlength]="500"></ion-textarea>
        <div>
          <ion-input label="Image" labelPlacement="floating" placeholder="image url" [formControl]="imageFC"></ion-input>
        </div>
      </ion-content>
      <ion-button (click)="updatePost()">Update Post</ion-button>
    `,
})
export class EditPostModal implements OnInit
{

  copyOfPost: PostModel;
  textFC = new FormControl("",[Validators.required, Validators.maxLength(500), Validators.minLength(3)]);
  imageFC = new FormControl("");

  post = new FormGroup(
    {
      text: this.textFC,
      imgurl: this.imageFC,
    }
  );
  constructor(
    public modalController: ModalController,
    public state: Globalstate,
    public toast: ToastController,
    public http: HttpClient,
    public popoverCtrl: PopoverController,
    public router: Router
  )
  {
    this.copyOfPost = this.state.currentPost;
    this.router.events.subscribe(event =>    {
      if(event instanceof NavigationStart) {
        this.ngOnInit();
      }
    })
  }
  ngOnInit()
  {
    this.textFC.patchValue(this.state.currentPost.text);
    this.imageFC.patchValue(this.state.currentPost.imgUrl);
  }
  async dismissModal() {
    while(await this.popoverCtrl.getTop())
      await this.popoverCtrl.dismiss();
    this.modalController.dismiss();
  }

  async updatePost() {
    try{
      const call = this.http.put<CommentModel>(environment.baseURL+"post/" + this.copyOfPost.id, this.post.value);
      const result = await firstValueFrom<CommentModel>(call);
      let index = this.state.posts.findIndex(c => c.id == this.copyOfPost.id)
      this.state.posts[index] = result as PostModel;
      this.state.currentPost = result as PostModel;


      (await this.toast.create({
        color: "success",
        message: 'post successfully updated.',
        duration: 2000,
      })).present();
      while(await this.popoverCtrl.getTop())
        await this.popoverCtrl.dismiss();
      this.dismissModal();

    }
    catch (e)
    {
      (await this.toast.create(
        {
          message: 'Failed to update post',
          color: "danger",
          duration: 2000,
        })).present();
    }
  }
}
