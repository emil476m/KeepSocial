import {Component, OnInit} from "@angular/core";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {Rooms} from "../models/Rooms.model";
import {environment} from "../../environments/environment.prod";

@Component({
  selector: 'app-room',
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
                      @room #{{room.rom_id}}
                  </ion-text>
                  <ion-title></ion-title>
                  <ion-button style="margin-left: 1.5%" (click)="goToChat(room.rom_id, room.rom_name)">
                      <ion-icon name="chevron-forward-outline"></ion-icon>
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
          <ion-button class="bottomBtn" style="margin-left: 3%" [disabled]="roomList.length<9" (click)="nextPage()">
              <p>&#160;</p>
              <ion-icon name="arrow-forward-outline"></ion-icon>
              <p>&#160;</p>
          </ion-button>
      </ion-item>



  `,
  styleUrls: ['room.page.scss'],
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

  goToChat(number: number, Roomname:string) {
    this.router.navigate(['chat/' + number + '/' + Roomname])
  }


  async getRooms() {
    try {
      const call = this.http.get<Rooms[]>(environment.baseURL+"Rooms?pageNumber=" + this.page);
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
