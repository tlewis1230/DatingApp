import {Injectable} from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User> {

    // need access to decoded token, so we bring in authService
    constructor (private userService: UserService, private authService: AuthService,
        private router: Router, private alertify: AlertifyService) {}
    // resolver - when we resolve, we go out to the user service
    //  get the user that matches the route parameters we're aiming to get - the rest of it is to catch the error if we have a problem
    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving your data.');
                this.router.navigate(['/members']);
                // return an observable of null
                return of(null);
            })
        );
    }
}
