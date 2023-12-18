import { Component } from "@angular/core";
import {TokenService} from "./services/token.service";
import {Globalstate} from "./services/states/globalstate";
import {Account, SimpleUser} from "./accountInterface";
import {environment} from "../environments/environment.prod";
import {firstValueFrom} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Component({
  template: `
      <ion-tabs>
          <ion-tab-bar slot="bottom">
              <ion-tab-button tab="home">
                  <ion-icon name="home-outline"></ion-icon>
                  Home
              </ion-tab-button>

            <ion-tab-button tab="myfeed" [disabled]="!token.getToken()">
              <ion-icon name="library-outline"></ion-icon>
              My Feed
            </ion-tab-button>

              <ion-tab-button tab="Chats" routerLink="rooms" [disabled]="!token.getToken()">
                <ion-icon name="chatbubbles-outline"></ion-icon>
                    Chat
              </ion-tab-button>

              <ion-tab-button tab="friendsList" [disabled]="!token.getToken()" routerLink="friends">
                  <ion-icon name="people-outline"></ion-icon>
                  Friend
              </ion-tab-button>
            <ion-tab-button tab="profile" routerLink="profile/{{state.currentUserName}}" [disabled]="!token.getToken()">
              <ion-icon name="person-outline"></ion-icon>
              Profile
            </ion-tab-button>
            <ion-tab-button tab="search" routerLink="search" [disabled]="!token.getToken()">
              <ion-icon name="search-outline"></ion-icon>
              Search
            </ion-tab-button>
              <ion-tab-button tab="account" routerLink="account" [disabled]="!token.getToken()">
                  <ion-icon name="settings-outline"></ion-icon>
                  Settings
              </ion-tab-button>

          </ion-tab-bar>
      </ion-tabs>
  `
})
export class TabsComponent
{
  constructor(public readonly token: TokenService, public readonly state :Globalstate, private http : HttpClient,) {
    if(this.state.currentUserName == null) this.whoAmI();
  }

  async whoAmI()
  {
    if (this.state.currentUserName != null) return;

    if(this.token.getToken())
    {
        const call = this.http.get<SimpleUser>(environment.baseURL+"account/simpleuser");
        const result = await firstValueFrom<SimpleUser>(call);
        this.state.currentUserName = result.userDisplayname;
    }
  }
}
