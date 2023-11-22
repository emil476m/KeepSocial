import {Component, OnInit} from "@angular/core";
import {Router} from "@angular/router";

@Component({
  selector: 'app-chat',
  template: `
      <ion-header [translucent]="true">
          <ion-toolbar>
              <ion-title>
                  Chat Rooms
              </ion-title>
          </ion-toolbar>
      </ion-header>

      <ion-content [fullscreen]="true">
          <ion-content>
              <ion-card>
                  <ion-title>
                      Room Tittle
                  </ion-title>
                  <ion-text>
                      @user
                  </ion-text>
                  <ion-title></ion-title>
                  <ion-button (click)="goToChat(101)">
                      <ion-icon name="chevron-forward-outline"></ion-icon>
                  </ion-button>
              </ion-card>
          </ion-content>
      </ion-content>

  `,
  styleUrls: ['chat.page.scss'],
})
export class RoomsPage implements OnInit {
  ngOnInit(): void {
  }

  constructor(private router:Router) {
  }

  goToChat(number: number) {
    this.router.navigate(['chat/'+number])
  }
}
