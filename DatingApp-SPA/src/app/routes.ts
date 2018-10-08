import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';
import { MemberListResolver } from './_resolvers/member-list-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';

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
    { path: 'members', component: MemberListComponent,
      resolve: {users: MemberListResolver}},
    { path: 'members/:id', component: MemberDetailComponent,
       resolve: {user: MemberDetailResolver}},
    { path: 'member/edit', component: MemberEditComponent,
       resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnsavedChanges]},
    { path: 'messages', component: MessagesComponent},
    { path: 'lists', component: ListsComponent},
  ]
},

{ path: '**', redirectTo: '', pathMatch: 'full'}
];
