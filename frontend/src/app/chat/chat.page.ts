import {Component, OnInit, ViewChild} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {FormControl, Validators} from "@angular/forms";
import {firstValueFrom} from "rxjs";
import {IonContent} from "@ionic/angular";
import {Message} from "../models/Message.model";
import {ActivatedRoute} from "@angular/router";
import {environment} from "../../environments/environment.prod";

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

      <ion-content #textWindow id="Textcontainer" [scrollEvents]="true" (ionScroll)="onScroll($event)">

        <ion-card id="textCard" *ngFor="let message of messages"
                  [ngClass]="{'left-card': !message.isSender, 'right-card': message.isSender}">
          <ion-tab-bar [ngStyle]="{ 'background-color': message.isSender ? '#001087' : '#3A3B3C' }">
            <ion-text style="margin-left: 1%">{{ message.User_id }}</ion-text>
            <ion-title style="color: White">{{ message.message }}</ion-title>
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
export class ChatPage implements OnInit {
  constructor(private http: HttpClient, public route: ActivatedRoute) {
  }

  displayname: string = "@displayname";
  chatroom_num: number = -1;
  @ViewChild('textWindow', {static: false}) textWindow!: IonContent;

  page: number = 1;

  message: FormControl<string | null> = new FormControl("", [Validators.required, Validators.minLength(3), Validators.maxLength(50)]);

  messages: Message[] = [];

  ngOnInit() {
    this.messages = [];
    this.page = 1;
    this.displayname = "@displayname";

    //document.getElementById("Textcontainer")!.scrollTop()
    this.route.params.subscribe(params => {
      const id = params['id'];
      const name = params['room_name'];
      this.chatroom_num = id;
      this.displayname = name;
    });

    this.getMessages();

    setTimeout(() => {
      this.textWindow.scrollToBottom();
    }, 1000);
  }

  async sendMessage() {
    if (this.message.value != null) {
      let text: Message = {
        room_id: this.chatroom_num, //TODO change this later
        message: this.message.value,
        isSender: true,
        User_id: -1,
        sendAt: "2023-11-21T10:11:49.028866"
        //user: {userId: 0, userDisplayName: "@me", userEmail: "email", userBirthday: "22:22:22", AvatarUrl:"", isDeleted:false}
      }

      try {
        const call = this.http.post<Message>(environment.baseURL+"SenndMessage", text)
        const reault = await firstValueFrom(call);
        this.message.reset()
        this.messages.push(reault)
      } catch (error) {
      }
    }
    setTimeout(() => {
      this.textWindow.scrollToBottom();
    }, 500);
  }

  async getMessages() {
    try {
      const call = this.http.get<Message[]>(environment.baseURL+"ChatMessages" + this.chatroom_num + "?pageNumber=" + this.page);
      this.page = this.page + 1;
      //this.messages = await firstValueFrom<Message[]>(call);
      call.subscribe((resData: Message[]) => {
        this.messages.unshift(...resData);
      })
    } catch (error) {
    }
  }

  onScroll(event: CustomEvent) {
    const scrollTop = event.detail.scrollTop;
    if (scrollTop === 0) {
      this.getMessages();
    }
  }

}
