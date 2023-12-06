import {Component, OnInit} from "@angular/core";
import {ModalController, PopoverController, ToastController} from "@ionic/angular";
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {CommentModel} from "../../models/CommentModel";
import {Globalstate} from "../../services/states/globalstate";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment.prod";
import {HttpClient} from "@angular/common/http";
import {NavigationStart, Route, Router} from "@angular/router";
import {logoFacebook} from "ionicons/icons";

@Component({
  template:
    `
      <ion-header>
        <ion-toolbar>
          <ion-title mode="ios">Edit comment</ion-title>
          <ion-buttons slot="end">
            <ion-button (click)="dismissModal()">Close</ion-button>
          </ion-buttons>
        </ion-toolbar>
      </ion-header>
      <ion-content>
        <ion-textarea label="comment" labelPlacement="floating" [formControl]="textFC" [counter]="true" [maxlength]="500"></ion-textarea>
        <div>
          <ion-img
            *ngIf="imgUrl && !imageChangedEvent"
            [src]="imgUrl"
            style="margin-left: 25%; margin-right: 25%; "
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
      </ion-content>
      <ion-button (click)="updateComment()">Update comment</ion-button>
    `,
})
export class EditCommentModal implements OnInit
{

  copyOfComment: CommentModel;
  textFC = new FormControl("",[Validators.required, Validators.maxLength(500), Validators.minLength(3)]);
  imageFC = new FormControl(this.imgUrl);


  file = new FormControl(null);

  imgUrl?: string;
  imageChangedEvent: Event | undefined;
  eventchange: Event|null = null;
  comment = new FormGroup(
    {
      text: this.textFC,
      imgurl: this.imageFC,
    }
  );

  img = this.fb.group(
    {
      image: [null as Blob | null],
    }
  )

  constructor(
    public modalController: ModalController,
    public state: Globalstate,
    public toast: ToastController,
    public http: HttpClient,
    public popoverCtrl: PopoverController,
    public router: Router,
    public fb: FormBuilder
  )
  {
    this.copyOfComment = this.state.currentComment;
    this.router.events.subscribe(event =>    {
      if(event instanceof NavigationStart) {
        this.ngOnInit();
      }
    })
  }
  ngOnInit()
  {
    this.textFC.patchValue(this.state.currentComment.text);
    this.imageFC.patchValue(this.state.currentComment.imgUrl);
    this.imgUrl = this.state.currentComment.imgUrl;
  }
  async dismissModal() {
    while(await this.popoverCtrl.getTop())
      await this.popoverCtrl.dismiss();
    this.modalController.dismiss();
  }

  async updateComment() {
    try{
      if (this.eventchange) {
        this.updateimg(this.eventchange)
      }
    const call = this.http.put<CommentModel>(environment.baseURL+"comment/" + this.copyOfComment.id, this.comment.value);
    const result = await firstValueFrom<CommentModel>(call);
    let index = this.state.comments.findIndex(c => c.id == this.copyOfComment.id)
    this.state.comments[index] = result as CommentModel;
    location.reload();

    (await this.toast.create({
      color: "success",
      message: 'comment successfully updated.',
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
          message: 'Failed to update comment',
          color: "danger",
          duration: 2000,
        })).present();
    }
  }

  async updateimg($event: Event) {
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
