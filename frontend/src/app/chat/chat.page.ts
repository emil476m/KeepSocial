import {Component} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {FormControl, Validators} from "@angular/forms";
import {firstValueFrom} from "rxjs";

@Component({
  selector: 'app-chat',
  template: `
    <ion-header [translucent]="true">
      <ion-toolbar>
        <ion-title>
          {{displayname}}
        </ion-title>
      </ion-toolbar>
    </ion-header>

    <ion-content [fullscreen]="true">
      <ion-header collapse="condense">
        <ion-toolbar>
          <ion-title size="large">Blank</ion-title>
        </ion-toolbar>
      </ion-header>


      <ion-content id="Textcontainer" [scrollEvents]="true" (ionScroll)="onScroll($event)">
        <ion-card id="textCard" *ngFor="let message of messages"
                  [ngClass]="{'left-card': !message.isSender, 'right-card': message.isSender}">
          <ion-tab-bar [ngStyle]="{ 'background-color': message.isSender ? '#001087' : '#870000' }">
            <ion-text style="margin-left: 1%">{{ message.User_id }}</ion-text>
            <ion-title>{{ message.message }}</ion-title>
          </ion-tab-bar>
        </ion-card>
      </ion-content>
    </ion-content>
    <ion-item>
      <ion-input placeholder="  text...  " [formControl]="message" id="messageInput"></ion-input>
      <ion-button (click)="sendMessage()" id="button" slot="end">
        <ion-icon name="send-outline"></ion-icon>
        <p>&#160; send message</p>
      </ion-button>
    </ion-item>
  `,
  styleUrls: ['chat.page.scss'],
})
export class ChatPage {
  displayname: string = "@displayname"

  page: number = 1;

  message: FormControl<string | null> = new FormControl("", [Validators.required,Validators.minLength(3),Validators.maxLength(50)]);

  messages: Message[] = [];

  constructor(private http: HttpClient) {
    this.getMessages();
  }

  async sendMessage() {
    console.log(this.message.value)
    if (this.message.value != null) {
      let sendMessage: Message = {
        room_id: 101,
        message: this.message.value,
        isSender: true,
        User_id: 0,
        sendAt: ""
        //user: {userId: 0, userDisplayName: "@me", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}

      }

      try {
        const call = this.http.post<Message>("http://localhost:5000/SenndMessage", this.message.value)
        const reault = await firstValueFrom(call);
        this.message.reset()
        this.messages.push(reault)
      } catch (error) {
      }
    }

  }

  async getMessages(){
    try {
      const call = this.http.get<Message[]>("http://localhost:5000/ChatMessages" + 101 + "?pageNumber=" + this.page);
      this.page = this.page + 1;
      //this.messages = await firstValueFrom<Message[]>(call);
      call.subscribe((resData: Message[]) => {
         this.messages.unshift(...resData);
      })
    } catch (error){}
  }

  onScroll(event: CustomEvent) {
    const scrollTop = event.detail.scrollTop;
    if (scrollTop === 0) {
      this.getMessages();
    }
  }

}

export interface Message {
  room_id: number;
  message: string;
  isSender: boolean;
  User_id: number;
  sendAt: string;

}

export interface User {
  userId: number;
  userDisplayName: string;
  userEmail: string;
  userBirthday: string;
  AvatarUrl: string;
  isDeleted: boolean;
}
