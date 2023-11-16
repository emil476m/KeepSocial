import {Component} from "@angular/core";
import {FormControl, Validators} from "@angular/forms";

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


      <div id="Textcontainer">
        <ion-card id="textCard" *ngFor="let message of messages"
                  [ngClass]="{'left-card': !message.isSender, 'right-card': message.isSender}">
          <ion-tab-bar [ngStyle]="{ 'background-color': message.isSender ? '#001087' : '#870000' }">
            <ion-text style="margin-left: 1%">{{ message.user.userDisplayName.toString() }}</ion-text>
            <ion-title>{{ message.message }}</ion-title>
          </ion-tab-bar>
        </ion-card>
      </div>

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

  message: FormControl<string | null> = new FormControl("", [Validators.required,Validators.minLength(3),Validators.maxLength(50)]);

  messages: Message[] = [
    {
    message: "Mock",
    isSender: false,
    user: {userId: 0, userDisplayName: "@user", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}
    },
    {
      message: "Mock",
      isSender: true,
      user: {userId: 0, userDisplayName: "@me", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}
    },
    {
      message: "Mock",
      isSender: false,
      user: {userId: 0, userDisplayName: "@user", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}
    },
    {
      message: "Mock",
      isSender: true,
      user: {userId: 0, userDisplayName: "@me", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}
    },
    {
      message: "Mock",
      isSender: false,
      user: {userId: 0, userDisplayName: "@user", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}
    },
    {
      message: "Mock",
      isSender: true,
      user: {userId: 0, userDisplayName: "@me", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}
    },

  ];

  constructor() {

    let testMessage: Message = {
      message: "Holla freind",
      isSender: false,
      user: {userId: 0, userDisplayName: "@user", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}
    }

    this.messages.push(testMessage);
  }

  sendMessage(){
    console.log(this.message.value)
    if (this.message.value != null) {
      let sendMessage: Message = {
        message: this.message.value,
        isSender: true,
        user: {userId: 0, userDisplayName: "@me", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}
      }
      this.messages.push(sendMessage)
    }
    this.message.reset()
  }

}

export interface Message {
  message: string;
  isSender: boolean;
  user: User;
}

export interface User {
  userId: number;
  userDisplayName: string;
  userEmail: string;
  userBirthday: string;
  AvatarUrl: string;
  isDeleted: boolean;
}
