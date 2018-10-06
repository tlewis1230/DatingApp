import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';


@Injectable({
  providedIn: 'root'
})
// will tell our route whether or not it's allowed or can activate the troute that's trying to be accessed
// pipe is union type
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router,
           private alertify: AlertifyService) {}

canActivate(): boolean {
    if (this.authService.loggedIn()) {
      return true;
    }
    this.alertify.error('You shall not pass!!!');
    this.router.navigate(['/home']);
    return false;
  }
}
