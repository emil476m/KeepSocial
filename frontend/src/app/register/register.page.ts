import {Component, OnInit} from "@angular/core";

@Component({
  selector: 'app-boxdetailed',
  styleUrls: ['register.css'],
  template:
    `


      <ion-content>

        <ion-header>
          <ion-toolbar>
            <ion-title align="center">Register Account</ion-title>
          </ion-toolbar>
        </ion-header>

        <ion-grid>
          <ion-row>
            <ion-col>
              <ion-item >
              <ion-input label-placement="floating" label="name" ></ion-input>
              </ion-item>
              <ion-item >
                <ion-input label-placement="floating" label="mail" ></ion-input>
              </ion-item>
              <ion-item >
                <ion-input label-placement="floating" label="Year" ></ion-input>
                <ion-input label-placement="floating" label="Month" ></ion-input>
                <ion-input label-placement="floating" label="Day" ></ion-input>
              </ion-item>
              <ion-item >
                <ion-input label-placement="floating" label="password" ></ion-input>
              </ion-item>
              <ion-item >
                <ion-input label-placement="floating" label="Repeat password" ></ion-input>
              </ion-item>

            </ion-col>

            <ion-col>
              <ion-grid >
                <ion-row >
                  <ion-col class="grid-item">
                    <ion-img class="profile-img" style="width: 30%" src='https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png'/>
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
export class RegisterPage{

  constructor() {

  }





}
