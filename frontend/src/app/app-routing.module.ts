import {NgModule} from '@angular/core';
import {PreloadAllModules, RouterModule, Routes} from '@angular/router';
import {HomePage} from './home/home.page';
import {ChatPage} from './chat/chat.page';
import {RoomsPage} from './chat/rooms.page';
import {TabsComponent} from "./tabs.component";
import {LoginComponent} from "./login/login.component";
import {RegisterPage} from "./register/register.page";
import {AccountPage} from "./account/account.page";
import {PostDetail} from "./PostDetailed/post.page";
import {FriendsPage} from "./freind/friends.page";
import {ProfilePage} from "./profile/profile.page";
import {MyfeedPage} from "./home/MyfeedPage/Myfeed.page";
import {SearchPage} from "./search/search.page";
import {TermsOfServicePage} from "./TermsofSerivcePage/termsOfService.page";

const routes: Routes = [
  {
    path: '',
    component: TabsComponent,
    children: [
      {
        path: "home",
        component: HomePage,
      },
      {
        path: "myfeed",
        component: MyfeedPage,
      },
      {

        path: "chat/:id/:room_name",
        component: ChatPage,
      },
      {
        path: "rooms",
        component: RoomsPage,
      },
      {
        path: "friends",
        component: FriendsPage,
      },
      {
        path: "account",
        component: AccountPage,
      },
      {
        path: "post/:id",
        component: PostDetail,
      },
      {
        path: '',
        redirectTo: '/home',
        pathMatch: 'full',
      },
      {
        path: "profile/:profileName",
        component: ProfilePage,
      },
      {
        path: 'search',
        component: SearchPage,
      },
      {
        path: 'search/:searchTerm',
        component: SearchPage,
      }
    ]
  },
  {
    path: "login",
    component: LoginComponent,
  },
  {
    path: "register",
    component: RegisterPage,
  },
  {
    path: "account",
    component: AccountPage,
  },
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full',
  },
  {
    path: "termsofservice",
    component: TermsOfServicePage,
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {preloadingStrategy: PreloadAllModules})
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
