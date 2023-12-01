import { Component } from "@angular/core";
import {TokenService} from "./services/token.service";

@Component({
  template: `
      <ion-tabs>
          <ion-tab-bar slot="bottom">
              <ion-tab-button tab="home">
                  <ion-icon name="home-outline"></ion-icon>
                  Home
              </ion-tab-button>

              <ion-tab-button tab="Chats" routerLink="rooms" [disabled]="!token.getToken()">
                <ion-icon name="chatbubbles-outline"></ion-icon>
                    Chat
              </ion-tab-button>

              <ion-tab-button tab="friendsList" [disabled]="!token.getToken()" routerLink="friends">
                  <ion-icon name="people-outline"></ion-icon>
                  Friend
              </ion-tab-button>

              <ion-tab-button tab="posts" [disabled]="!token.getToken()">
                  <ion-icon name="library-outline"></ion-icon>
                  Posts
              </ion-tab-button>
              <ion-tab-button tab="account" routerLink="account" [disabled]="!token.getToken()">
                  <ion-icon name="settings-outline"></ion-icon>
                  Settings
              </ion-tab-button>
            <ion-tab-button tab="profile" routerLink="profile" [disabled]="!token.getToken()">
              <ion-icon name="person-outline"></ion-icon>
              Profile
            </ion-tab-button>
          </ion-tab-bar>
      </ion-tabs>
  `
})
export class TabsComponent
{
  constructor(public readonly token: TokenService) {
  }
}
