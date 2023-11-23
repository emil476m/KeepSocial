import {Component, OnInit} from "@angular/core";
import {Router} from "@angular/router";
import {Message} from "../models/Message.model";
import {HttpClient} from "@angular/common/http";
import {Rooms} from "../models/Rooms.model";

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
              <ion-card *ngFor="let room of roomList">
                  <ion-title style="margin-left: 0.2%">
                      {{room.rom_name}}
                  </ion-title>
                  <ion-text style="margin-left: 1.5%">
                      @user {{room.rom_id}}
                  </ion-text>
                  <ion-title></ion-title>
                  <ion-button style="margin-left: 1.5%" (click)="goToChat(room.rom_id)">
                      <ion-icon name="chevron-forward-outline"></ion-icon>
                  </ion-button>
              </ion-card>

              <ion-item>
                  <ion-button style="margin-left: 40%" [disabled]="page<2" (click)="previousPage()">Previous page</ion-button>
                  <ion-button style="margin-left: 3%"[disabled]="roomList.length<9" (click)="nextPage()">Next page</ion-button>
              </ion-item>
          </ion-content>
      </ion-content>


  `,
  styleUrls: ['chat.page.scss'],
})
export class RoomsPage implements OnInit {

  page: number = 1;
  upcomingPage: number = 2;
  roomList: Rooms[] = [];


  ngOnInit(): void {
  }

  constructor(private router: Router, private http: HttpClient) {
    this.getRooms()
  }

  goToChat(number: number) {
    this.router.navigate(['chat/' + number])
  }


  async getRooms() {
    try {
      const call = this.http.get<Rooms[]>("http://localhost:5000/Rooms?pageNumber=" + this.page);
      call.subscribe((resData: Rooms[]) => {
        this.roomList = resData;
      })
    } catch (error) {
    }
  }

  async nextPage() {
    this.page = this.page + 1;
    await this.router.navigate(['/'], {queryParams: {page: this.page, resultsPerPage: this.upcomingPage}});
    this.getRooms();

  }

  async previousPage() {
    this.page = this.page - 1;
    await this.router.navigate(['/'], {queryParams: {page: this.page, resultsPerPage: this.upcomingPage}});
    this.getRooms();
  }
}
