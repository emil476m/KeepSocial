import {Component, OnInit} from "@angular/core";
import {Account} from "../accountInterface";
import {firstValueFrom, window} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Component({
  template:
    `
      <ion-content>
        <ion-header>
          <ion-toolbar>
            <ion-title align="center">My Account Settings</ion-title>
          </ion-toolbar>
        </ion-header>
        <ion-grid>
          <ion-row>
            <ion-col>
              <ion-item>
                <ion-list-header>Account Info</ion-list-header>
              </ion-item>
              <ion-item >
              <ion-label>DisplayName:</ion-label>
                <ion-label [textContent]="AName"></ion-label>
                <ion-button>Change</ion-button>
              </ion-item>
              <ion-item >
                <ion-label>Email:</ion-label>
                <ion-label [textContent]="AEmail"></ion-label>
                <ion-button>Change</ion-button>
              </ion-item>
              <ion-item >
                <ion-label >Birthday:</ion-label>
                <ion-label [textContent]="ADate"></ion-label>
                <ion-button (click)="s()" fill="clear"  >ㅤㅤㅤㅤㅤ</ion-button>
              </ion-item>
              <ion-item >
                <ion-label>Password:</ion-label>
                <ion-label>********</ion-label>
                <ion-button>Change</ion-button>
              </ion-item>
            </ion-col>
            <ion-col>
              <ion-grid >
                <ion-row >
                  <ion-col class="grid-item">
                    <ion-img class="profile-img" style="width: 30%" [src]="currentAvatarUrl"/>
                  </ion-col>
                </ion-row>
                <ion-row>
                  <ion-col class="grid-item">
                    <ion-button>
                      Change Picture
                    </ion-button>
                  </ion-col>
                </ion-row>
              </ion-grid>
            </ion-col>
          </ion-row>
        </ion-grid>
      </ion-content>
        `,
})

export class AccountPage implements OnInit{


  defaultAvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png";
  currentAvatarUrl = this.defaultAvatarUrl;

  AName = "";
  AEmail = "";
  ADate = "";
  constructor(private http: HttpClient) {
  }

  ngOnInit() {
    this.getAccountInfo()
  }

  async getAccountInfo(){
    const call = this.http.get<Account>("http://localhost:5000/api/whoami");
    const result = await firstValueFrom<Account>(call);
    console.log("this is the user return: " + result);
    this.AName = result.userDisplayName
    this.AEmail = result.userEmail;
    var myDate = new Date(result.userBirthday);
    this.ADate = myDate.getDate() + "\\" +  (myDate.getMonth()+1) + "\\" + myDate.getFullYear();
    console.log("this is name variable: " + this.ADate)
  }
  async s(){
    this.window(location.href="https://www.youtube.com/watch?v=xvFZjo5PgG0");
  }

  async changeUserDisplayName(){

  }
  async changePassword(){

  }

  async changeProfilePicture(){

  }
  async changeEmail(){

  }

  protected readonly window = window;
}
