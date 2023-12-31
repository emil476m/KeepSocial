import {Component, OnInit} from "@angular/core";
import {Account} from "../accountInterface";
import {firstValueFrom, window} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {ModalController, ToastController} from "@ionic/angular";
import {FormControl, Validators} from "@angular/forms";
import {NewAccountInfoModal} from "../changeAccountInfoModal/AccountInfoModal";
import {Globalstate} from "../services/states/globalstate";
import {NavigationStart, Router} from "@angular/router";

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
              <ion-item lines="none" >
              <ion-label>DisplayName:</ion-label>
                <ion-label [textContent]="AName.value"></ion-label>

                <ion-button (click)="changeUserDisplayName()" [textContent]="BtnNameText"></ion-button>
              </ion-item>
              <ion-item >
                <ion-label>Email:</ion-label>
                <ion-label [textContent]="AEmail"></ion-label>
                <ion-button (click)="changeEmail()">Change</ion-button>
              </ion-item>
              <ion-item >
                <ion-label >Birthday:</ion-label>
                <ion-label [textContent]="ADate"></ion-label>
                <ion-button (click)="s()" fill="clear"  >ㅤㅤㅤㅤㅤ</ion-button>
              </ion-item>
              <ion-item >
                <ion-label>Password:</ion-label>
                <ion-label>********</ion-label>
                <ion-button (click)="changePassword()">Change</ion-button>
              </ion-item>
            </ion-col>
            <ion-col>
              <ion-grid >
                <ion-row >
                  <ion-col class="grid-item">
                    <ion-avatar  style="width: 200px; height: 200px;" >
                      <img alt="Silhouette of a person's head" [src]="currentAvatarUrl"  />
                    </ion-avatar>
                  </ion-col>
                </ion-row>
                <ion-row>
                  <ion-col class="grid-item">
                    <ion-card>
                      <ion-input
                        label="Avatar"
                        type="file"
                        accept="image/png, image/jpeg"
                        (change)="onFileChanged($event)"
                      ></ion-input>
                    </ion-card>
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

  INameMode = true;
  BtnNameText = "Change";

  defaultAvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png";
  currentAvatarUrl: string = "";

  AName = new FormControl("",[Validators.required,Validators.minLength(1),Validators.maxLength(100)]);
  AEmail = "";
  ADate = "";
  constructor(private http: HttpClient, public toastControl: ToastController, private modalcontroller: ModalController, private globalstate: Globalstate, public router: Router)
  {
    this.router.events.subscribe(event =>    {
      if(event instanceof NavigationStart) {
        this.getAccountInfo();
      }})
    }

  protected readonly window = window;

  openEdit() {

    this.modalcontroller.create({
      component: NewAccountInfoModal,
      componentProps: {
      }
    }).then(res => {
      res.present();
    })
  }

  isEnabled = false;
  setIsDisabled(value: boolean, inputvariable: FormControl) {
this.isEnabled = !this.isEnabled;
  }

  ngOnInit() {
    this.getAccountInfo()
  }

  async getAccountInfo(){
    const call = this.http.get<Account>(environment.baseURL+"whoami");
    const result = await firstValueFrom<Account>(call);
    this.AName.setValue(result.userDisplayName);
    this.AEmail = result.userEmail;
    var myDate = new Date(result.userBirthday);
    this.ADate = myDate.getDate() + "\\" +  (myDate.getMonth()+1) + "\\" + myDate.getFullYear();
    this.currentAvatarUrl = result.avatarUrl
    if (this.currentAvatarUrl == null || this.currentAvatarUrl == "") this.currentAvatarUrl = this.defaultAvatarUrl;
  }
  async s(){
    this.window(location.href="https://www.youtube.com/watch?v=xvFZjo5PgG0");
  }

  async changeUserDisplayName(){
    this.globalstate.updatingWhatAccountItem="Account Name";
    this.openEdit();

  }
  async changePassword(){
    this.globalstate.updatingWhatAccountItem="Account Password";
    this.openEdit();
  }

  async changeProfilePicture(){
    this.globalstate.updatingWhatAccountItem="Account Avatar";
    this.openEdit();
  }

  async onFileChanged($event: Event) {
    // The event contains all/any selected files
    const files = ($event.target as HTMLInputElement).files;
    if (!files) return;
    const file = files[0];
    const formData = new FormData();
    formData.set("avatar", file);
    await firstValueFrom(this.http.put(environment.baseURL + 'account/updateAvatar', formData));
    await location.reload();
  }

  async changeEmail(){
    this.globalstate.updatingWhatAccountItem="Account Email";
    this.openEdit();

  }
}

export interface AccountUpdate {
  text: string;
}
