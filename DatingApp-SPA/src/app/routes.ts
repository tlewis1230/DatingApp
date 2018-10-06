import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';

// array of routes
// path & component
// first match wins
// want to redirect someone to home page if we come in under another tab
// if we type in a bogus address this will also redirect us back to the home page
export const appRoutes: Routes = [
{ path: '', component: HomeComponent},
{
  path: '', // by leaving this empty, we're going to match on //locahost:4200/members
  runGuardsAndResolvers: 'always',
  canActivate: [AuthGuard],
  children: [
    { path: 'members', component: MemberListComponent},
    { path: 'messages', component: MessagesComponent},
    { path: 'lists', component: ListsComponent},
  ]
},

{ path: '**', redirectTo: '', pathMatch: 'full'}
];
