import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';

import {JwtHelperService} from '@auth0/angular-jwt';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  jwtHelper  = new JwtHelperService();

  constructor (private authService: AuthService) {}
  ngOnInit() {
    const token = localStorage.getItem('token');
    // turn string into object
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.authService.currentUser = user;
      // any component that subscribes to currentPhotoUrl in authService
      //  is also going to get the updated photo once the applciation refreshes
      this.authService.changeMemberPhoto(user.photoUrl);
    }

  }
}
