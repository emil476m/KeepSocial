import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {RouteReuseStrategy} from '@angular/router';

import {IonicModule, IonicRouteStrategy} from '@ionic/angular';

import {AppComponent} from './app.component';
import {AppRoutingModule} from './app-routing.module';
import {HomePage} from "./home/home.page";
import {TabsComponent} from "./tabs.component";
import {ErrorHttpInterceptor} from "../interceptors/error-http-interceptors";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {TokenService} from "./services/token.service";
import {AuthHttpInterceptor} from "../interceptors/auth-http-interceptor";
import {LoginComponent} from "./login/login.component";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {NgxCaptchaModule} from "ngx-captcha";
import {ReCapchaV3Service} from "./services/reCapchaV3.service";
import {RegisterPage} from "./register/register.page";
import {AccountService} from "./services/account.service";
import {ChatPage} from "./chat/chat.page";
import {RoomsPage} from "./chat/rooms.page";

import {AccountPage} from "./account/account.page";
import {FriendsPage} from "./freind/friends.page";
import {NewAccountInfoModal} from "./changeAccountInfoModal/AccountInfoModal";
import {PostDetail} from "./PostDetailed/post.page";
import {ProfilePage} from "./profile/profile.page";
import {EditCommentModal} from "./PostDetailed/EditCommentModal/edit.comment.modal";
import {EditPostModal} from "./PostDetailed/EditPostModal/edit.post.modal";
import {SearchPage} from "./search/search.page";
import {FollowerslistModal} from "./profile/FollowersList/Followerslist.modal";
import {FollowingListModal} from "./profile/followingList/followingList.modal";
import {MyfeedPage} from "./home/MyfeedPage/Myfeed.page";
import {TermsOfServicePage} from "./TermsofSerivcePage/termsOfService.page";

@NgModule({
  declarations:
    [AppComponent,
      HomePage,
      TabsComponent,
      LoginComponent,
      RegisterPage,
      AccountPage,
      ChatPage,
      RoomsPage,
      NewAccountInfoModal,
      PostDetail,
      FriendsPage,
      EditCommentModal,
      EditPostModal,
      ProfilePage,
      FollowerslistModal,
      FollowingListModal,
      MyfeedPage,
      SearchPage,
      TermsOfServicePage
  ],
    imports:
        [BrowserModule,
            IonicModule.forRoot(),
            AppRoutingModule,
            ReactiveFormsModule,
            NgxCaptchaModule,
            HttpClientModule, FormsModule,
        ],
  providers: [{provide: RouteReuseStrategy, useClass: IonicRouteStrategy},
    {provide: HTTP_INTERCEPTORS, useClass: ErrorHttpInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true},
    TokenService,
    ReCapchaV3Service,
    AccountService,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
}
