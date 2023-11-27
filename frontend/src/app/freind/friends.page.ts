import {Component, OnInit} from "@angular/core";
import {Rooms} from "../models/Rooms.model";
import {User} from "../models/User.model";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment.prod";

@Component({
  selector: 'friend',
  template: `
    <ion-header [translucent]="true">
      <ion-toolbar>
        <ion-title>
          Friend-List
        </ion-title>
      </ion-toolbar>
    </ion-header>

    <ion-content [fullscreen]="true">
      <ion-content>
        <ion-card *ngFor="let friend of friendliest">
          <ion-title style="margin-left: 0.2%">
            {{friend.userDisplayName}}
          </ion-title>
          <ion-text style="margin-left: 1.5%">
            @user #{{friend.userId}}
          </ion-text>
          <ion-title></ion-title>
          <ion-button class="cardBtn1" (click)="openChat(friend.userId)"><!--Open chat with friend or create if they dont have one-->
            <ion-icon name="chatbubble-ellipses-outline"></ion-icon>
          </ion-button>
          <ion-button class="cardBtn" (click)="goFriend(friend.userId)"><!---view acccount--->
            <ion-icon name="person-outline"></ion-icon>
          </ion-button>
        </ion-card>
      </ion-content>
    </ion-content>

    <ion-item>
      <ion-button class="bottomBtn" style="margin-left: 40%" [disabled]="page<2" (click)="previousPage()">
        <p>&#160;</p>
        <ion-icon name="arrow-back-outline"></ion-icon>
        <p>&#160;</p>
      </ion-button>
      <ion-button class="bottomBtn" style="margin-left: 3%" [disabled]="friendliest.length<9" (click)="nextPage()">
        <p>&#160;</p>
        <ion-icon name="arrow-forward-outline"></ion-icon>
        <p>&#160;</p>
      </ion-button>
    </ion-item>
  `,
  styleUrls: ['friends.page.scss']
})

export class FriendsPage implements OnInit {
  page: number = 1;
  upcomingPage: number = 2;

  friendliest: User[] = [];

  ngOnInit(): void {
  }
  constructor(private router: Router, private http: HttpClient) {
    this.getFriends()

  }

  async getFriends() {
    try {
      const call = this.http.get<User[]>(environment.baseURL+"freinds?pageNumber=" + this.page);
      call.subscribe((resData: User[]) => {
        this.friendliest = resData;
      })
    } catch (error) {
    }
  }

  async nextPage() {
    this.page = this.page + 1;
    await this.router.navigate(['/'], {queryParams: {page: this.page, resultsPerPage: this.upcomingPage}});
    //get more users
  }

  async previousPage() {
    this.page = this.page - 1;
    await this.router.navigate(['/'], {queryParams: {page: this.page, resultsPerPage: this.upcomingPage}});
    //get more users
  }

  openChat(userId: number) {
    console.log("writing to #" + userId);

  }

  goFriend(userId: number) {
    console.log("viewing user #" + userId);
    //implement when user profile have been made
  }
}
